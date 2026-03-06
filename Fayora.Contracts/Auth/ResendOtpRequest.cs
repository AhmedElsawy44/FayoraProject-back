namespace Fayora.Contracts.Auth;

public record ResendOtpRequest(string? Email, string? PhoneNumber, string DeviceId, string OtpPurpose);
