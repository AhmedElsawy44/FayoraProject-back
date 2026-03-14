namespace Fayora.Application.Features.AuthModule.Commands.LoginWithApple;

public record LoginWithAppleResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);