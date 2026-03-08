namespace Fayora.Contracts.Auth.Responses;

public record EmailVerifyResponse(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
