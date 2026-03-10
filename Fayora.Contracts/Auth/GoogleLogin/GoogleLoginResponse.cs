namespace Fayora.Contracts.Auth.GoogleLogin;

public record GoogleLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
