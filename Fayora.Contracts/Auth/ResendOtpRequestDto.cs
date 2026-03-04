namespace Fayora.Contracts.Auth;

public record ResendOtpRequestDto(Guid UserId, string? Email, string? PhoneNumber, string OtpPurpose);
