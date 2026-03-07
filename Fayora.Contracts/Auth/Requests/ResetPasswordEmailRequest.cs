namespace Fayora.Contracts.Auth.Requests;

public record ResetPasswordEmailRequest(
    string Email,
    string ResetToken,
    string NewPassword
);
