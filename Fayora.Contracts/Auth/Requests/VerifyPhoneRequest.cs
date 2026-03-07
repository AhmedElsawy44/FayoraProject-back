namespace Fayora.Contracts.Auth.Requests;

public record VerifyPhoneRequest(
Guid UserId,
string PhoneNumber,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
