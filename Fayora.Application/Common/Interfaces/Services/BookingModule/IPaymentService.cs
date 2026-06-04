using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Common.Interfaces.Services.BookingModule;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> GeneratePaymentUrlAsync(PaymentRequest request, CancellationToken cancellationToken = default);
    Result<WebhookResult> ValidateAndParseWebhook(string jsonPayload, string receivedHmac);
    Task<Result<Success>> RefundAsync(string gatewayTransactionId, decimal amountInEgp, CancellationToken cancellationToken = default);
}

public record PaymentRequest(
    Guid BookingId,
    decimal AmountInEgp,
    string CustomerFirstName,
    string CustomerLastName,
    string? CustomerEmail,
    string? CustomerPhoneNumber,
    PaymentMethodType MethodType,
    string? WalletNumber
);

public record PaymentResponse(
    string PaymentUrl,
    string GatewayOrderId
);

public record WebhookResult(
    bool IsSuccess,
    string GatewayOrderId,
    string BookingId,
    decimal AmountCents
);
