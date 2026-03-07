namespace Fayora.Contracts.Auth.Responses;

public record VerifyEmailResponse(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
