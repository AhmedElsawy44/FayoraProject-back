namespace Fayora.Contracts.Auth;

public record ResetPasswordRequest(
    string? Email,
    string? PhoneNumber,
    string DeviceId,
    string ResetPasswordToken,
    string NewPassword);
