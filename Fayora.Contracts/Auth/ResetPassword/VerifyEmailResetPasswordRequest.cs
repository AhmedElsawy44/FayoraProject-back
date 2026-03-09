namespace Fayora.Contracts.Auth.ResetPassword;

public record VerifyEmailResetPasswordRequest(
    string Email,
    string Code);
