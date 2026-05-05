using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Interfaces.Services.BookingModule;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> GeneratePaymentUrlAsync(PaymentRequest request, CancellationToken cancellationToken = default);
    Result<WebhookResult> ValidateAndParseWebhook(IReadOnlyDictionary<string, string> webhookData, string receivedHmac);
}

public record PaymentRequest(
    Guid BookingId,
    decimal AmountInEgp,
    string CustomerFirstName,
    string CustomerLastName,
    string? CustomerEmail,
    string? CustomerPhoneNumber,
    PaymentMethodType MethodType
);

public record PaymentResponse(
    string PaymentUrl,
    string GatewayOrderId
);

public enum PaymentMethodType
{
    OnlineCard,
    MobileWallet
}

public record WebhookResult(
    bool IsSuccess,
    string GatewayOrderId,
    string BookingId,
    decimal AmountCents
);
