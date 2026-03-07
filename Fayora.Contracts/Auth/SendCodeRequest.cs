namespace Fayora.Contracts.Auth;

public record SendCodeRequest(string? Email, string? PhoneNumber, string DeviceId, string OtpPurpose, CodeDeliveryMethod DeliveryMethod = CodeDeliveryMethod.Email);

public enum CodeDeliveryMethod
{
    Email,
    Sms,
    Whatsapp
}
