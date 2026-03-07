namespace Fayora.Contracts.Auth;

public record SendCodeRequest(string? Email, string? PhoneNumber, string DeviceId, string OtpPurpose);
