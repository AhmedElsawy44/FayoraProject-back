namespace Fayora.Contracts.Auth.Responses;

public record PhoneVerifyResponse(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);

