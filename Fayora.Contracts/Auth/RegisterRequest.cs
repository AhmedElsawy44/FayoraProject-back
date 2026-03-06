namespace Fayora.Contracts.Auth;

public record RegisterRequest(string? Email, string? PhoneNumber, string Password, string DeviceId);