namespace Fayora.Contracts.Auth.Requests;

public record VerifyEmailRequest(
Guid UserId,
string Email,
string Code,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage);
