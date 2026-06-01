using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Domain.Entities.Booking;

public class PaymentTransaction
{
    public Guid BookingId { get; init; }
    public string GatewayOrderId { get; init; } = default!;
    public decimal Amount { get; init; }
    public PaymentMethodType PaymentMethod { get; init; }
    public PaymentTransactionStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? ErrorMessage { get; private set; }

    public PaymentTransaction(Guid bookingId, string gatewayOrderId, decimal amount, PaymentMethodType paymentMethod)
    {
        BookingId = bookingId;
        GatewayOrderId = gatewayOrderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PaymentTransactionStatus.Pending;
    }

    private PaymentTransaction() { }

    public void MarkAsPaid(string gatewayTransactionId)
    {
        Status = PaymentTransactionStatus.Paid;
        GatewayTransactionId = gatewayTransactionId;
        ErrorMessage = null;
    }
    public void MarkAsPartiallyPaid(string gatewayTransactionId)
    {
        Status = PaymentTransactionStatus.PartiallyPaid;
        GatewayTransactionId = gatewayTransactionId;
    }

    public void MarkAsFailed(string errorMessage, string? gatewayTransactionId = null)
    {
        Status = PaymentTransactionStatus.Failed;
        ErrorMessage = errorMessage;
        GatewayTransactionId = gatewayTransactionId;
    }


    public Result<Success> MarkAsRefunded()
    {
        if (Status != PaymentTransactionStatus.Paid && Status != PaymentTransactionStatus.PartiallyPaid)
        {
            return Error.Validation("Only paid or partially paid transactions can be refunded.");
        }
        Status = PaymentTransactionStatus.Refunded;
        return Result.Success;
    }
}
