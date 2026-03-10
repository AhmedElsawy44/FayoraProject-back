namespace Fayora.Contracts.Auth.FacebookLogin;

public record FacebookLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
