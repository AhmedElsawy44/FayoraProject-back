namespace Fayora.Application.Features.AuthModule.Commands.LoginWithGoogle;

public record LoginWithGoogleResult
(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

