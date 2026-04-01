namespace Fayora.Contracts.AuthModule.GoogleLogin;

public record GoogleLoginResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ImageUrl,
    bool IsFirstLogin,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
