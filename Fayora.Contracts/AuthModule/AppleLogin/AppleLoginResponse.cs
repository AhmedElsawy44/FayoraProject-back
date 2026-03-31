namespace Fayora.Contracts.AuthModule.AppleLogin;

public record AppleLoginResponse
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
