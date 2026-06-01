using Fayora.Domain.Entities.Booking;

namespace Fayora.Application.Common.Interfaces.Persistences.BookingModule;

public interface IPaymentTransactionRepository
{
    Task<PaymentTransaction?> GetByBookingGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default);
    Task<PaymentTransaction?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    void AddPaymentTransaction(PaymentTransaction transaction);
}
