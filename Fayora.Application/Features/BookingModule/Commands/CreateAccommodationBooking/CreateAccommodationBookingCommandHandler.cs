using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.TourGuideModule;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.BookingModule.Commands.CreateAccommodationBooking
{
    public class CreateAccommodationBookingCommandHandler(
        IUserRepository userRepository,
        IHousingUnitRepository housingUnitRepository,
        IBookingRepository bookingRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        ICalendarBlockRepository calendarBlockRepository,
        IClientContextProvider clientContextProvider,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateAccommodationBookingCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            CreateAccommodationBookingCommand request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var user = await userRepository.GetUserByIdAsync(
                userId, new UserQueryOptions { IsReadOnly = true }, cancellationToken);
            if (user is null) return AuthErrors.UserNotFound;

            var unit = await housingUnitRepository.GetUnitByIdAsync(
                request.UnitId, new UnitQueryOptions { IsReadOnly = true }, cancellationToken);
            if (unit is null) return AccommodationErrors.UnitNotFound;

            // Chaeck if the unit is active
            if (unit.Status != ItemStatus.Active)
                return AccommodationErrors.UnitNotAvailable;

            var startDateTime = request.StartDate.ToDateTime(TimeOnly.FromTimeSpan(unit.CheckInTime));
            var endDateTime = request.EndDate.ToDateTime(TimeOnly.FromTimeSpan(unit.CheckOutTime));

            // check that the unit is not already booked or blocked in the requested dates
            var hasOverlap = await bookingRepository.HasOverlapAsync(
                unit.Id, startDateTime, endDateTime, cancellationToken);
            if (hasOverlap) return AccommodationErrors.UnitNotAvailable;

            // check if the number of guests exceeds the maximum allowed
            int totalGuests = request.Adults + request.Children;
            if (totalGuests > unit.MaxGuests)
                return AccommodationErrors.ExceedsMaxGuests;

            //// calculate the total price

            //int nights = (request.EndDate.DayNumber - request.StartDate.DayNumber);
            //decimal totalPrice = unit.PricePerNight * nights;
            //decimal serviceFee = totalPrice * unit.CommissionRate; // a 20% service fee (20% عمولة الشركه)
            //decimal payoutAmount = totalPrice - serviceFee;

            int nights = request.EndDate.DayNumber - request.StartDate.DayNumber;
            var pricingResult = unit.CalculatePricing(nights);
            if (pricingResult.IsError) return pricingResult.Errors;

            var (totalPrice, serviceFee, payoutAmount) = (
                pricingResult.Value.TotalPrice,
                pricingResult.Value.ServiceFee,
                pricingResult.Value.PayoutAmount);


            // create the booking 
            var booking = Booking.Create(
                userId,
                unit.OwnerId,
                ServiceType.Accommodation,
                unit.Id,
                totalPrice,
                serviceFee,
                payoutAmount,
                totalGuests,
                unit.CancellationPolicy,
                startDateTime,
                endDateTime,
                request.IsCashOnArrival);
            if (booking.IsError) return booking.Errors;

            // create the calendar block for the booked dates to prevent double booking 
            var calendarBlock = new CalendarBlock(
                unit.Id,
                ServiceType.Accommodation,
                startDateTime,
                endDateTime,
                BlockReason.Booked,
                booking.Value.Id);

            // save the booking and calendar block
            bookingRepository.AddBooking(booking.Value);
            calendarBlockRepository.AddCalendarBlock(calendarBlock);
            await unitOfWork.CommitChangesAsync(cancellationToken);


            // لو العميل اختار الدفع عند الوصول، هيدفع العربون بس دلوقتي، ولو اختار يدفع أونلاين هيدفع السعر كامل
            decimal amountToPay = request.IsCashOnArrival
                ? booking.Value.DepositAmount
                : booking.Value.TotalPrice;

            // then send to payment service
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
                // Compensation - Remove the booking and calendar block if payment URL generation fails to avoid having orphaned bookings without payment
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
