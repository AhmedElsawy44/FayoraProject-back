namespace Fayora.Contracts.Auth.Login;

public record PhoneLoginRequest(
    string PhoneNumber,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
