using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.SharedModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;

public class CreatePackageBookingCommandHandler(
    IUserRepository userRepository,
    IBookingRepository bookingRepository,
    IPackageRepository packageRepository,
    IPackageOccurrenceRepository packageOccurrenceRepository,
    IDiscountOfferRepository discountOfferRepository,
    IPaymentTransactionRepository paymentTransactionRepository,
    IClientContextProvider clientContextProvider,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePackageBookingCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreatePackageBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = true }, cancellationToken);
        if (user is null) return AuthErrors.UserNotFound;

        var package = await packageRepository.GetPackageByIdAsync(request.PackageId, new PackageQueryOptions { ReadOnly = true }, cancellationToken);
        if (package is null) return TourGuideErrors.PackageNotFound;

        if (package.PackageStatus != Fayora.Domain.Enums.TourGuideModule.ItemStatus.Active)
            return TourGuideErrors.PackageNotAvailable;

        var occurrence = await
            packageOccurrenceRepository.GetOccurrenceByPackageIdAndDate(request.PackageId, request.BookingDate, cancellationToken);
        if (occurrence is null) return BookingErrors.OccurrenceNotFound;

        int requiredSpots = request.Adults + request.Children;
        var reserveResult = occurrence.ReserveSeats(requiredSpots);
        if (reserveResult.IsError) return reserveResult.Errors;

        var totalPrice = package.CalculateBooking(request.Adults, request.Children);
        if (totalPrice.IsError) return totalPrice.Errors;


        decimal serviceFee = totalPrice.Value * 0m; // = 0% service fee, can be changed later if needed
        decimal payoutAmount = totalPrice.Value - serviceFee;



        Guid? appliedOfferId = null;
        decimal discountAmount = 0;

        var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
            package.Id, OfferTargetType.GuidePackage, cancellationToken);

        var offer = activeOffers.FirstOrDefault();
        if (offer is not null)
        {
            var discountResult = offer.ApplyTo(totalPrice.Value);
            if (!discountResult.IsError)
            {
                discountAmount = totalPrice.Value - discountResult.Value;
                appliedOfferId = offer.Id;

                var discountedBasePrice = discountResult.Value;
                serviceFee = discountedBasePrice * 0m;
                payoutAmount = discountedBasePrice - serviceFee;
            }
        }

        var booking = Booking.Create(
            userId,
            package.UserId,
            ServiceType.GuidePackage,
            package.Id,
            totalPrice.Value,
            serviceFee,
            payoutAmount,
            request.Adults,
            request.Children,
            package.CancellationPolicy,
            request.BookingDate.ToDateTime(TimeOnly.MinValue),
            request.BookingDate.ToDateTime(TimeOnly.MinValue).AddHours(package.DurationHours),
            request.IsCashOnArrival,
            appliedOfferId,
            discountAmount);
        if (booking.IsError) return booking.Errors;


        bookingRepository.AddBooking(booking.Value);
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
            occurrence.ReleaseSeats(requiredSpots);
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return paymentResult.Errors;
        }


        // save the payment transaction with gatewayOrderId and pending status, it will be updated later by the payment webhook
        paymentTransactionRepository.AddPaymentTransaction(new PaymentTransaction(
            booking.Value.Id,
            paymentResult.Value.GatewayOrderId,
            amountToPay,
            request.PaymentMethodType));
        await unitOfWork.CommitChangesAsync(cancellationToken);


        return paymentResult.Value.PaymentUrl;
    }
}
