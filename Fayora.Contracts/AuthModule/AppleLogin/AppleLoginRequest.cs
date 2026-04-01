namespace Fayora.Contracts.AuthModule.AppleLogin;

public record AppleLoginRequest(
    string IdToken,
    string FirstName,
    string LastName,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
