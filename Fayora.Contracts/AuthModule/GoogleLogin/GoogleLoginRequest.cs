namespace Fayora.Contracts.AuthModule.GoogleLogin;

public record GoogleLoginRequest(
string AccessToken,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage
);
