namespace Fayora.Contracts.Auth.Requests;

public record PhoneLoginRequest(
    string PhoneNumber,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
