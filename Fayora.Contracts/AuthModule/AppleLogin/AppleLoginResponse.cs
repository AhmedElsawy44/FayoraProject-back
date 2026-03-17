namespace Fayora.Contracts.AuthModule.AppleLogin;

public record AppleLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
