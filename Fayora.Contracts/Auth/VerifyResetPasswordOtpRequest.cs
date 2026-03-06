namespace Fayora.Contracts.Auth;

public record VerifyResetPasswordOtpRequest(
    string? Email,
    string? PhoneNumber,
    string DeviceId,
    string Code
);