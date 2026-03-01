namespace Fayora.Contracts.Auth;

public record RegisterRequest(string FirstName, string LastName, string? Email, string? PhoneNumber, string Password, string? SimCountryIsoCode, string TimeZone, DeviceInfoDto DeviceInfo);

public record DeviceInfoDto(string DeviceId, string FcmToken, string DeviceLanguage);
