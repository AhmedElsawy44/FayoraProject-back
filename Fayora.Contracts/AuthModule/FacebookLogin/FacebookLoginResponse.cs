namespace Fayora.Contracts.AuthModule.FacebookLogin;

public record FacebookLoginResponse
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
