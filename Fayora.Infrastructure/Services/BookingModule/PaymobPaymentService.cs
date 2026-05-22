using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Infrastructure.Settings;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Fayora.Infrastructure.Services.BookingModule;

public class PaymobPaymentService(HttpClient httpClient, PaymobSettings paymobSettings) : IPaymentService
{
    public async Task<Result<PaymentResponse>> GeneratePaymentUrlAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        int amountInCents = (int)(request.AmountInEgp * 100);

        var authResponse = await httpClient.PostAsJsonAsync("auth/tokens",
            new
            {
                api_key = paymobSettings.ApiKey

            }, cancellationToken);

        var authData = await authResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        string authToken = authData.GetProperty("token").GetString()!;

        var orderResponse = await httpClient.PostAsJsonAsync("ecommerce/orders",
            new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = amountInCents,
                currency = "EGP",
                items = Array.Empty<object>()
            }, cancellationToken);

        var orderData = await orderResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var gatewayOrderId = orderData.GetProperty("id").GetRawText();


        var integrationId = request.MethodType == PaymentMethodType.MobileWallet
            ? paymobSettings.WalletIntegrationId : paymobSettings.CardIntegrationId;

        var paymentKeyResponse = await httpClient.PostAsJsonAsync("acceptance/payment_keys",
            new
            {
                auth_token = authToken,
                amount_cents = amountInCents,
                expiration = 3600,
                order_id = gatewayOrderId,
                billing_date = new
                {
                    apartment = "NA",
                    email = request.CustomerEmail,
                    floor = "NA",
                    first_name = request.CustomerFirstName,
                    street = "NA",
                    building = "NA",
                    phone_number = request.CustomerPhoneNumber,
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "NA",
                    country = "EG",
                    last_name = request.CustomerLastName,
                    state = "NA"
                },
                currency = "EGP",
                integration_id = integrationId
            }, cancellationToken);


        var paymentKeyData = await paymentKeyResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        string paymentToken = paymentKeyData.GetProperty("token").GetString()!;


        string paymentUrl;
        
        if (request.MethodType == PaymentMethodType.MobileWallet)
        {
            var walletResponse = await httpClient.PostAsJsonAsync("acceptance/payments/pay",
                new
                {
                    source = new
                    {
                        identifier = request.WalletNumber,
                        subtype = "WALLET"
                    },
                    payment_token = paymentToken
                }, cancellationToken);

            var walletData = await walletResponse.Content
                .ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            paymentUrl = walletData.GetProperty("redirect_url").GetString()!;
        }
        else
        {
            paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{paymobSettings.IframeId}?payment_token={paymentToken}";
        }

        return new PaymentResponse(
            paymentUrl,
            gatewayOrderId);
    }

    public Result<WebhookResult> ValidateAndParseWebhook(IReadOnlyDictionary<string, string> webhookData, string receivedHmac)
    {
        var keysToHash = new[]
        {
            "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
            "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
            "is_standalone_payment", "is_voided", "order", "owner", "pending", "source_data.pan",
            "source_data.sub_type", "source_data.type", "success"
        };

        var concatenatedString = new StringBuilder();
        foreach (var key in keysToHash)
        {
            if (webhookData.TryGetValue(key, out var value))
            {
                concatenatedString.Append(value);
            }
        }

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(paymobSettings.HmacSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString.ToString()));
        var calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        if (calculatedHmac != receivedHmac.ToLower())
        {
            return Error.Failure("Invalid HMAC signature. Possible forgery attack!");
        }

        bool isSuccess = bool.Parse(webhookData["success"]);
        string gatewayOrderId = webhookData["order"];
        decimal amountCents = decimal.Parse(webhookData["amount_cents"]);

        string bookingId = webhookData.TryGetValue("order.merchant_order_id", out var bId) ? bId : string.Empty;

        var result = new WebhookResult(isSuccess, gatewayOrderId, bookingId, amountCents);
        return result;
    }
}
