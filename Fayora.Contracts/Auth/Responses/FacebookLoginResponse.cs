namespace Fayora.Contracts.Auth.Responses;

public record FacebookLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
