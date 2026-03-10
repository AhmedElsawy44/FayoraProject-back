namespace Fayora.Contracts.Auth.GoogleLogin;

public record GoogleLoginRequest(
string AccessToken,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage
);
