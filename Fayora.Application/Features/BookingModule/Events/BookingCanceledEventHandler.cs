using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Events.BookingModule;
using Fayora.Domain.Enums.BookingModule;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.BookingModule.Events
{
    public class BookingCanceledEventHandler(
        IBookingRepository bookingRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        ICalendarBlockRepository calendarBlockRepository,
        IPackageOccurrenceRepository packageOccurrenceRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        ILogger<BookingCanceledEventHandler> logger)
        : INotificationHandler<BookingCanceledEvent>
    {
        public async Task Handle(BookingCanceledEvent notification, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetBookingByIdAsync(
                notification.BookingId, cancellationToken);
            if (booking is null) return;


            await calendarBlockRepository.RemoveByBookingIdAsync(
                booking.Id, cancellationToken);


            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                await packageOccurrenceRepository.ReleaseSeatsAsync(
                    booking.ServiceId,
                    DateOnly.FromDateTime(booking.StartDate),
                    booking.SeatsCount,
                    cancellationToken);
            }


            var paymentTransaction = await paymentTransactionRepository
                .GetByBookingIdAsync(booking.Id, cancellationToken);

            if (paymentTransaction is not null &&
                paymentTransaction.Status == PaymentTransactionStatus.Paid)
            {
                var refundResult = await paymentService.RefundAsync(
                    paymentTransaction.GatewayTransactionId!,
                    paymentTransaction.Amount,
                    cancellationToken);

                if (!refundResult.IsError)
                {
                    paymentTransaction.MarkAsRefunded();
                    booking.MarkAsRefunded();
                }
                else
                {
                    logger.LogError("Failed to refund paid booking {BookingId}. Gateway Transaction ID: {GatewayTransactionId}. Error: {Error}",
                        booking.Id,
                        paymentTransaction.GatewayTransactionId,
                        refundResult.Errors.FirstOrDefault()?.Description);
                }
            }
            else if (paymentTransaction is not null &&
                     paymentTransaction.Status == PaymentTransactionStatus.PartiallyPaid)
            {
                var refundResult = await paymentService.RefundAsync(
                    paymentTransaction.GatewayTransactionId!,
                    booking.DepositAmount,
                    cancellationToken);

                if (!refundResult.IsError)
                {
                    paymentTransaction.MarkAsRefunded();
                    booking.MarkAsRefunded();
                }
                else
                {
                    logger.LogError("Failed to refund partially paid booking deposit {BookingId}. Gateway Transaction ID: {GatewayTransactionId}. Error: {Error}",
                        booking.Id,
                        paymentTransaction.GatewayTransactionId,
                        refundResult.Errors.FirstOrDefault()?.Description);
                }
            }

            await unitOfWork.CommitChangesAsync(cancellationToken);
        }
    }
}
