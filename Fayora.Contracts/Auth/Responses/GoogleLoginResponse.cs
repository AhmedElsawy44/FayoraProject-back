namespace Fayora.Contracts.Auth.Responses;

public record GoogleLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
