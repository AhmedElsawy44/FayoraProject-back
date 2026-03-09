namespace Fayora.Contracts.Auth.ResetPassword;

public record ResetEmailPasswordRequest(
    string Email,
    string ResetToken,
    string NewPassword
);
