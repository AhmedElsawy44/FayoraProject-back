namespace Fayora.Contracts.Auth.Responses;

public record LoginEmailResponse(
    string UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
