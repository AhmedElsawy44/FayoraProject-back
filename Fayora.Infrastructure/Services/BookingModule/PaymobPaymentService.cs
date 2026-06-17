using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Fayora.Infrastructure.Services.BookingModule;

public class PaymobPaymentService(HttpClient httpClient, IOptions<PaymobSettings> paymobSettingsOptions) : IPaymentService
{
    private readonly PaymobSettings paymobSettings = paymobSettingsOptions.Value;
    public async Task<Result<PaymentResponse>> GeneratePaymentUrlAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        int amountInCents = (int)(request.AmountInEgp * 100);

        var authResponse = await httpClient.PostAsJsonAsync("auth/tokens",
            new
            {
                api_key = paymobSettings.ApiKey

            }, cancellationToken);

        if (!authResponse.IsSuccessStatusCode)
        {
            var errorContent = await authResponse.Content.ReadAsStringAsync(cancellationToken);
            return Error.Failure("Payment.AuthFailed", $"Paymob authentication failed: {errorContent}");
        }

        var authData = await authResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (!authData.TryGetProperty("token", out var tokenProp))
        {
            return Error.Failure("Payment.AuthFailed", "Paymob authentication token not found in response.");
        }
        string authToken = tokenProp.GetString()!;

        var orderResponse = await httpClient.PostAsJsonAsync("ecommerce/orders",
            new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = amountInCents,
                currency = "EGP",
                merchant_order_id = request.BookingId.ToString(),
                items = Array.Empty<object>()
            }, cancellationToken);

        if (!orderResponse.IsSuccessStatusCode)
        {
            var errorContent = await orderResponse.Content.ReadAsStringAsync(cancellationToken);
            return Error.Failure("Payment.OrderCreationFailed", $"Paymob order creation failed: {errorContent}");
        }

        var orderData = await orderResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (!orderData.TryGetProperty("id", out var idProp))
        {
            return Error.Failure("Payment.OrderCreationFailed", "Paymob order ID not found in response.");
        }
        var gatewayOrderId = idProp.GetRawText();


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

        if (!paymentKeyResponse.IsSuccessStatusCode)
        {
            var errorContent = await paymentKeyResponse.Content.ReadAsStringAsync(cancellationToken);
            return Error.Failure("Payment.KeyGenerationFailed", $"Paymob payment key generation failed: {errorContent}");
        }

        var paymentKeyData = await paymentKeyResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (!paymentKeyData.TryGetProperty("token", out var tokenPropKey))
        {
            return Error.Failure("Payment.KeyGenerationFailed", "Paymob payment token not found in response.");
        }
        string paymentToken = tokenPropKey.GetString()!;


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

            if (!walletResponse.IsSuccessStatusCode)
            {
                var errorContent = await walletResponse.Content.ReadAsStringAsync(cancellationToken);
                return Error.Failure("Payment.WalletPaymentFailed", $"Paymob wallet payment initiation failed: {errorContent}");
            }

            var walletData = await walletResponse.Content
                .ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            if (!walletData.TryGetProperty("redirect_url", out var redirectUrlProp))
            {
                return Error.Failure("Payment.WalletPaymentFailed", "Paymob wallet redirect URL not found in response.");
            }
            paymentUrl = redirectUrlProp.GetString()!;
        }
        else
        {
            paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{paymobSettings.IframeId}?payment_token={paymentToken}";
        }

        return new PaymentResponse(
            paymentUrl,
            gatewayOrderId);
    }

    public async Task<Result<Success>> RefundAsync(string gatewayTransactionId, decimal amountInEgp, CancellationToken cancellationToken = default)
    {
        int amountInCents = (int)(amountInEgp * 100);

        var authResponse = await httpClient.PostAsJsonAsync("auth/tokens",
            new { api_key = paymobSettings.ApiKey }, cancellationToken);

        var authData = await authResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        string authToken = authData.GetProperty("token").GetString()!;

        var refundResponse = await httpClient.PostAsJsonAsync("acceptance/void_refund/refund",
            new
            {
                auth_token = authToken,
                transaction_id = gatewayTransactionId,
                amount_cents = amountInCents
            }, cancellationToken);

        return refundResponse.IsSuccessStatusCode
            ? Result.Success
            : Error.Failure("Refund.Failed", "Failed to process refund.");
    }

    public Result<WebhookResult> ValidateAndParseWebhook(string jsonPayload, string receivedHmac)
    {
        try
        {
            using var document = JsonDocument.Parse(jsonPayload);
            var root = document.RootElement;
            
            if (!root.TryGetProperty("obj", out var objElement))
            {
                return Error.Failure("Webhook.InvalidPayload", "Missing obj element in Paymob payload.");
            }

            var amount_cents = GetStringOrRaw(objElement, "amount_cents");
            var created_at = GetStringOrRaw(objElement, "created_at");
            var currency = GetStringOrRaw(objElement, "currency");
            var error_occured = GetStringOrRaw(objElement, "error_occured");
            var has_parent_transaction = GetStringOrRaw(objElement, "has_parent_transaction");
            var id = GetStringOrRaw(objElement, "id");
            var integration_id = GetStringOrRaw(objElement, "integration_id");
            var is_3d_secure = GetStringOrRaw(objElement, "is_3d_secure");
            var is_auth = GetStringOrRaw(objElement, "is_auth");
            var is_capture = GetStringOrRaw(objElement, "is_capture");
            var is_refunded = GetStringOrRaw(objElement, "is_refunded");
            var is_standalone_payment = GetStringOrRaw(objElement, "is_standalone_payment");
            var is_voided = GetStringOrRaw(objElement, "is_voided");
            
            string order = "";
            string merchantOrderId = "";
            if (objElement.TryGetProperty("order", out var orderElement))
            {
                if (orderElement.ValueKind == JsonValueKind.Object)
                {
                    order = GetStringOrRaw(orderElement, "id");
                    merchantOrderId = GetStringOrRaw(orderElement, "merchant_order_id");
                }
                else
                {
                    order = orderElement.GetRawText();
                }
            }

            var owner = GetStringOrRaw(objElement, "owner");
            var pending = GetStringOrRaw(objElement, "pending");

            string sourceDataPan = "";
            string sourceDataSubType = "";
            string sourceDataType = "";
            if (objElement.TryGetProperty("source_data", out var sdElement) && sdElement.ValueKind == JsonValueKind.Object)
            {
                sourceDataPan = GetStringOrRaw(sdElement, "pan");
                sourceDataSubType = GetStringOrRaw(sdElement, "sub_type");
                sourceDataType = GetStringOrRaw(sdElement, "type");
            }

            var success = GetStringOrRaw(objElement, "success");

            var concatenatedString = new StringBuilder();
            concatenatedString.Append(amount_cents);
            concatenatedString.Append(created_at);
            concatenatedString.Append(currency);
            concatenatedString.Append(error_occured);
            concatenatedString.Append(has_parent_transaction);
            concatenatedString.Append(id);
            concatenatedString.Append(integration_id);
            concatenatedString.Append(is_3d_secure);
            concatenatedString.Append(is_auth);
            concatenatedString.Append(is_capture);
            concatenatedString.Append(is_refunded);
            concatenatedString.Append(is_standalone_payment);
            concatenatedString.Append(is_voided);
            concatenatedString.Append(order);
            concatenatedString.Append(owner);
            concatenatedString.Append(pending);
            concatenatedString.Append(sourceDataPan);
            concatenatedString.Append(sourceDataSubType);
            concatenatedString.Append(sourceDataType);
            concatenatedString.Append(success);

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(paymobSettings.HmacSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString.ToString()));
            var calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            if (calculatedHmac != receivedHmac.ToLower())
            {
                return Error.Failure("Invalid HMAC signature. Possible forgery attack!");
            }

            bool isSuccessVal = false;
            if (objElement.TryGetProperty("success", out var successProp))
            {
                if (successProp.ValueKind == JsonValueKind.True) isSuccessVal = true;
                else if (successProp.ValueKind == JsonValueKind.False) isSuccessVal = false;
                else if (successProp.ValueKind == JsonValueKind.String) bool.TryParse(successProp.GetString(), out isSuccessVal);
            }

            decimal.TryParse(amount_cents, out var amountCentsDecimal);

            return new WebhookResult(isSuccessVal, order, merchantOrderId, amountCentsDecimal);
        }
        catch (Exception ex)
        {
            return Error.Failure("Webhook.ParseError", $"Error parsing webhook payload: {ex.Message}");
        }
    }

    private static string GetStringOrRaw(JsonElement element, string propName)
    {
        if (!element.TryGetProperty(propName, out var prop))
            return string.Empty;

        return prop.ValueKind switch
        {
            JsonValueKind.String => prop.GetString() ?? string.Empty,
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "null",
            _ => prop.GetRawText()
        };
    }
}
