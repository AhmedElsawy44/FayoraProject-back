namespace Fayora.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailResult(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
