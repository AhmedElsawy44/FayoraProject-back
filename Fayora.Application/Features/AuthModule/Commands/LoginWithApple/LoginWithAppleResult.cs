namespace Fayora.Application.Features.Auth.Commands.LoginWithApple;

public record LoginWithAppleResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken
);