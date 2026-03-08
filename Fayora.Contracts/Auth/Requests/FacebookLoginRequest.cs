namespace Fayora.Contracts.Auth.Requests;


public record FacebookLoginRequest(
    string AccessToken,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
