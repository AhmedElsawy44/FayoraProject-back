namespace Fayora.Contracts.Auth.Verify;

public record EmailVerifyRequest(
Guid UserId,
string Email,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
