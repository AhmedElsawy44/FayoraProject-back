namespace Fayora.Contracts.Auth.Login;

public record EmailLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
