namespace Fayora.Contracts.Auth.Responses;

public record EmailLoginResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
