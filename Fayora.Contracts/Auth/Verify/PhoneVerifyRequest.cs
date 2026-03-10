namespace Fayora.Contracts.Auth.Verify;

public record PhoneVerifyRequest(
Guid UserId,
string PhoneNumber,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
