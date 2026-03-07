namespace Fayora.Contracts.Auth.Requests;

public record SendPhoneCodeRequest(
    string PhoneNumber,
    string DeviceId,
    OtpPurpose Purpose,
    OtpDeliveryMethod DeliveryMethod
);

public enum OtpDeliveryMethod
{
    Sms,
    Whatsapp
}