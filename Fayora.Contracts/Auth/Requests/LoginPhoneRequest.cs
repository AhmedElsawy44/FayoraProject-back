namespace Fayora.Contracts.Auth.Requests;

public record LoginPhoneRequest(
    string PhoneNumber,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
