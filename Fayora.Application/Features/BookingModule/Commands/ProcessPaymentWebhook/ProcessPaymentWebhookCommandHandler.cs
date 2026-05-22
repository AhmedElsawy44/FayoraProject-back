using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using MediatR;
using System.Text.Json;

namespace Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook;

public class ProcessPaymentWebhookCommandHandler(
    IPaymentService paymentService,
    IBookingRepository bookingRepository,
    ICalendarBlockRepository calendarBlockRepository,
    IPackageOccurrenceRepository packageOccurrenceRepository,
    IPaymentTransactionRepository paymentTransactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ProcessPaymentWebhookCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var webHookData = JsonSerializer.Deserialize<IDictionary<string, string>>(request.JsonPayload)?.AsReadOnly();

        if (webHookData is null) return BookingErrors.InvalidPaymentWebhook;

        var validationResult = paymentService.ValidateAndParseWebhook(webHookData, request.ReceivedHmac);

        if (validationResult.IsError) return validationResult.Errors;

        var paymentInfo = validationResult.Value;

        if (!Guid.TryParse(paymentInfo.BookingId, out var parsedBookingId)) return BookingErrors.InvalidBookingId;

        var booking = await bookingRepository.GetBookingByIdAsync(parsedBookingId, cancellationToken);
        if (booking is null) return BookingErrors.BookingNotFound;

        var paymentTransaction = await paymentTransactionRepository.GetByBookingGatewayOrderIdAsync(paymentInfo.GatewayOrderId, cancellationToken);
        if (paymentTransaction is null) return BookingErrors.PaymentTransactionNotFound;

        if (paymentInfo.IsSuccess)
        {
            if (booking.IsCashOnArrival)
            {
                var depositResult = booking.MarkDepositAsPaid();
                if (depositResult.IsError) return depositResult.Errors;
                paymentTransaction.MarkAsPartiallyPaid(paymentInfo.GatewayOrderId);
            }
            else
            {
                var paidResult = booking.MarkAsPaid();
                if (paidResult.IsError) return paidResult.Errors;
                paymentTransaction.MarkAsPaid(paymentInfo.GatewayOrderId);
            }
        }
        else
        {
            var cancelResult = booking.Cancel("Payment Failed");
            if (cancelResult.IsError) return cancelResult.Errors;
            paymentTransaction.MarkAsFailed("Payment Failed", paymentInfo.GatewayOrderId);


            // remove  CalendarBlock
            await calendarBlockRepository.RemoveByBookingIdAsync(
                booking.Id, cancellationToken);

            // if it was package release seats
            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                await packageOccurrenceRepository.ReleaseSeatsAsync(
                    booking.ServiceId,
                    DateOnly.FromDateTime(booking.StartDate),
                    booking.SeatsCount,
                    cancellationToken);

            }

        }

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new Unit();
    }
}
