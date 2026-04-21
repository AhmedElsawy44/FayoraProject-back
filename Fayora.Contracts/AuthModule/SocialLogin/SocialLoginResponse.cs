namespace Fayora.Contracts.AuthModule.AppleLogin;

public record SocialLoginResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    bool IsFirstLogin,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
