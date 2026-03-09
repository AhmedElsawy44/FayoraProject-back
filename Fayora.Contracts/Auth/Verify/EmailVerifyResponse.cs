namespace Fayora.Contracts.Auth.Verify;

public record EmailVerifyResponse(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
