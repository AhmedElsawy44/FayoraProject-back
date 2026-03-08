namespace Fayora.Application.Features.Auth.Commands.LoginWithGoogle;

public record LoginWithGoogleResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

