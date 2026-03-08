namespace Fayora.Contracts.Auth.Requests;

public record ResetEmailPasswordRequest(
    string Email,
    string ResetToken,
    string NewPassword
);
