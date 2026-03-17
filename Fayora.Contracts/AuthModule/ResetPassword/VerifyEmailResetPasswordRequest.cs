namespace Fayora.Contracts.AuthModule.ResetPassword;

public record VerifyEmailResetPasswordRequest(
    string Email,
    string Code);
