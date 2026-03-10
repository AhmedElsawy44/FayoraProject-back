namespace Fayora.Contracts.Auth.AppleLogin;

public record AppleLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
