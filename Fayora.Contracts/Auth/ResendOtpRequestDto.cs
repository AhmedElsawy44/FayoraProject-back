namespace Fayora.Contracts.Auth;

public record ResendOtpRequestDto(string? Email, string? PhoneNumber, string OtpPurpose);
