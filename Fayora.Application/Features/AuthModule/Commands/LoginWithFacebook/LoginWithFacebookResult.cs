namespace Fayora.Application.Features.Auth.Commands.LoginWithFacebook;

public record LoginWithFacebookResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
