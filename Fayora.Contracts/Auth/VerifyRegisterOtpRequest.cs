namespace Fayora.Contracts.Auth;

public record VerifyRegisterOtpRequest(Guid UserId, string? Email, string? PhoneNumber, string Code, string DeviceId, string FcmToken, string? SimCountryIsoCode, string TimeZone, string DeviceLanguage);

