namespace Fayora.Contracts.AuthModule.ResetPassword;

public record ResetEmailPasswordRequest(
    string Email,
    string ResetToken,
    string NewPassword
);
