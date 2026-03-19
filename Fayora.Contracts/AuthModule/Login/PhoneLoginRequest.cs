namespace Fayora.Contracts.AuthModule.Login;

public record PhoneLoginRequest(
    string PhoneNumber,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
