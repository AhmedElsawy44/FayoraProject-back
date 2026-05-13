using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.BookingModule;

public class PaymentTransactionRepository(ApplicationDbContext context) : IPaymentTransactionRepository
{
    public void AddPaymentTransaction(PaymentTransaction transaction)
    {
        context.PaymentTransactions.Add(transaction);
    }

    public Task<PaymentTransaction?> GetByBookingGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default)
    {
        return context.PaymentTransactions.FirstOrDefaultAsync(x => x.GatewayOrderId == gatewayOrderId, cancellationToken);
    }
}

