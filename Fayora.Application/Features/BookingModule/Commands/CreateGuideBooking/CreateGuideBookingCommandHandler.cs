using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.BookingModule.Commands.CreateGuideBooking
{
    public class CreateGuideBookingCommandHandler(
        IUserRepository userRepository,
        ITourGuideRepository tourGuideRepository,
        IGuideWeeklyScheduleRepository scheduleRepository,
        IBookingRepository bookingRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        ICalendarBlockRepository calendarBlockRepository,
        IDiscountOfferRepository discountOfferRepository,
        IClientContextProvider clientContextProvider,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateGuideBookingCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            CreateGuideBookingCommand request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var user = await userRepository.GetUserByIdAsync(
                userId, new UserQueryOptions { IsReadOnly = true }, cancellationToken);
            if (user is null) return AuthErrors.UserNotFound;


            var guide = await tourGuideRepository.GetGuideByIdAsync(
                request.GuideId,
                new GuideQueryOptions { ReadOnly = true },
                cancellationToken);
            if (guide is null) return TourGuideErrors.GuideNotFound;

            if (guide.Status != ItemStatus.Active || !guide.IsAvailableForBooking)
                return TourGuideErrors.GuideNotAvailable;

            // Check if the guide is available to work on the requested day
            var dayOfWeek = request.BookingDate.DayOfWeek;
            var schedule = await scheduleRepository.GetByGuideIdAndDayAsync(
                request.GuideId, dayOfWeek, cancellationToken);
            if (schedule is null) return TourGuideErrors.GuideNotAvailableOnThisDay;

            // check if the guide has a booking on the requested date
            var hasBooking = await bookingRepository.HasGuideBookingOnDateAsync(
                request.GuideId, request.BookingDate, cancellationToken);
            if (hasBooking) return TourGuideErrors.GuideAlreadyBooked;


            // calculate the start and end date time of the booking
            var startDateTime = request.BookingDate.ToDateTime(TimeOnly.FromTimeSpan(schedule.StartTime));
            var endDateTime = request.BookingDate.ToDateTime(TimeOnly.FromTimeSpan(schedule.EndTime));

            // calculate the total price
            if (guide.BaseRate is null) return TourGuideErrors.GuideRateNotSet;
            if (guide.PricingUnit is null) return TourGuideErrors.GuideRateNotSet;

            int totalGuests = request.Adults + request.Children;
            double durationHours = (endDateTime - startDateTime).TotalHours;

            decimal totalPrice = guide.PricingUnit switch
            {
                PricingUnit.PerHour => guide.BaseRate.Value * (decimal)durationHours,
                PricingUnit.PerDay => guide.BaseRate.Value,
                PricingUnit.PerPerson => guide.BaseRate.Value * totalGuests,
                PricingUnit.PerTrip => guide.BaseRate.Value,
                _ => guide.BaseRate.Value
            };

            decimal serviceFee = totalPrice * 0.2m; //a 20% service fee (20% عمولة الشركه)
            decimal payoutAmount = totalPrice - serviceFee;


            Guid? appliedOfferId = null;
            decimal discountAmount = 0;

            var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                guide.UserId, OfferTargetType.TourGuide, cancellationToken);

            var offer = activeOffers.FirstOrDefault();
            if (offer is not null)
            {
                var discountResult = offer.ApplyTo(totalPrice);
                if (!discountResult.IsError)
                {
                    discountAmount = totalPrice - discountResult.Value;
                    appliedOfferId = offer.Id;
                }
            }

            // create the booking
            var booking = Booking.Create(
                userId,
                request.GuideId,
                ServiceType.TourGuide,
                request.GuideId,
                totalPrice,
                serviceFee,
                payoutAmount,
                request.Adults + request.Children,
                guide.CancellationPolicy,
                startDateTime,
                endDateTime,
                request.IsCashOnArrival,
                appliedOfferId,
                discountAmount);
            if (booking.IsError) return booking.Errors;

            // CalendarBlock for tour guide booking
            var calendarBlock = new CalendarBlock(
                request.GuideId,
                ServiceType.TourGuide,
                startDateTime,
                endDateTime,
                BlockReason.Booked,
                booking.Value.Id);

            bookingRepository.AddBooking(booking.Value);
            calendarBlockRepository.AddCalendarBlock(calendarBlock);
            await unitOfWork.CommitChangesAsync(cancellationToken);


            decimal amountToPay = request.IsCashOnArrival
                  ? booking.Value.DepositAmount
                  : booking.Value.TotalPrice;

            var paymentResult = await paymentService.GeneratePaymentUrlAsync(new PaymentRequest(
                booking.Value.Id,
                amountToPay,
                user.FirstName,
                user.LastName,
                user.PrimaryEmail?.Value,
                user.PhoneNumber?.Value,
                request.PaymentMethodType,
                request.WalletNumber));
            if (paymentResult.IsError)
            {
                bookingRepository.RemoveBooking(booking.Value);
                calendarBlockRepository.RemoveCalendarBlock(calendarBlock);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return paymentResult.Errors;
            }



            paymentTransactionRepository.AddPaymentTransaction(new PaymentTransaction(
                booking.Value.Id,
                paymentResult.Value.GatewayOrderId,
                totalPrice,
                request.PaymentMethodType));
            await unitOfWork.CommitChangesAsync(cancellationToken);



            return paymentResult.Value.PaymentUrl;
        }
    }
}
