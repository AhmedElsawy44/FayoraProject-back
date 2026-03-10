namespace Fayora.Contracts.Auth.Verify;

public record EmailVerifyRequest(
string Email,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
