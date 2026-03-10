namespace Fayora.Contracts.Auth.AppleLogin;

public record AppleLoginRequest(
    string IdToken,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
