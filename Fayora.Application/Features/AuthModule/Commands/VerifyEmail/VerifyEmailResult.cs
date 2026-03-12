namespace Fayora.Application.Features.AuthModule.Commands.VerifyEmail;

public record VerifyEmailResult(
    Guid Id,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
