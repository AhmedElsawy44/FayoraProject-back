namespace Fayora.Contracts.Auth.FacebookLogin;


public record FacebookLoginRequest(
    string AccessToken,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
