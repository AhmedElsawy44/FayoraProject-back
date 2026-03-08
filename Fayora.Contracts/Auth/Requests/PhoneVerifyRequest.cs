namespace Fayora.Contracts.Auth.Requests;

public record PhoneVerifyRequest(
Guid UserId,
string PhoneNumber,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
