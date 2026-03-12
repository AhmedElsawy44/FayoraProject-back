namespace Fayora.Contracts.AuthModule.Verify;

public record EmailVerifyResponse(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
