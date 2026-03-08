namespace Fayora.Contracts.Auth.Requests;

public record AppleLoginRequest(
    string IdToken,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
