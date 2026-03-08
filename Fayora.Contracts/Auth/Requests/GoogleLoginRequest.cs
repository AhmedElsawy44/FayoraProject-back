namespace Fayora.Contracts.Auth.Requests;

public record GoogleLoginRequest(
string AccessToken,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage
);
