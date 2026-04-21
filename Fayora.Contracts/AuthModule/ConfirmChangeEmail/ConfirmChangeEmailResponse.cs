namespace Fayora.Contracts.AuthModule.ConfirmChangeEmail;

public record ConfirmChangeEmailResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
