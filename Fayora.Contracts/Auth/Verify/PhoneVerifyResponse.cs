namespace Fayora.Contracts.Auth.Verify;

public record PhoneVerifyResponse(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);

