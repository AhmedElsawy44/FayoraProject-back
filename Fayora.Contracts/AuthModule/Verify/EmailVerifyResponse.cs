namespace Fayora.Contracts.AuthModule.Verify;

public record EmailVerifyResponse
(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
