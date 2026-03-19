namespace Fayora.Contracts.AuthModule.Verify;

public record PhoneVerifyRequest(
string PhoneNumber,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
