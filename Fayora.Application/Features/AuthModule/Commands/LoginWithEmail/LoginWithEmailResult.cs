namespace Fayora.Application.Features.Auth.Commands.LoginWithEmail;

public record LoginWithEmailResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);