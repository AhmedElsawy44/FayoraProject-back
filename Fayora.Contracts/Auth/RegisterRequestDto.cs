namespace Fayora.Contracts.Auth;

public record RegisterRequestDto(string? Email, string? PhoneNumber, string Password, string? SimCountryIsoCode, string TimeZone, DeviceInfoDto DeviceInfo);

public record DeviceInfoDto(string DeviceId, string DeviceLanguage);