namespace Fayora.Contracts.AuthModule.ConfirmChangeEmail;

public record ConfirmChangeEmailResponse
(
    Guid Id,
    string Email,
    string AccessToken,
    int ExpiresIn
);
