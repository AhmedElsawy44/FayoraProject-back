namespace Fayora.Contracts.AuthModule.ChangeEmail;

public record ChangeEmailResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);