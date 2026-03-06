namespace Fayora.Contracts.Auth;

public record LoginRequest(string? Email, string? PhoneNumber, string Password, string DeviceId, string FcmToken, string DeviceLanguage);
