namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public record LoginWithEmailResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);