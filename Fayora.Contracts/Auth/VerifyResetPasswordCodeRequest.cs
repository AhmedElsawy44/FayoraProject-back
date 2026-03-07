namespace Fayora.Contracts.Auth;

public record VerifyResetPasswordCodeRequest(
    string? Email,
    string? PhoneNumber,
    string DeviceId,
    string Code
);