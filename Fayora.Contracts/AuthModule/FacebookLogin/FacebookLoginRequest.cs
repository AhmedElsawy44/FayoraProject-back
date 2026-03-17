namespace Fayora.Contracts.AuthModule.FacebookLogin;


public record FacebookLoginRequest(
    string AccessToken,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
