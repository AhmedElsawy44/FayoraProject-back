namespace Fayora.Application.Features.AuthModule.Commands.LoginWithGoogle;

public record LoginWithGoogleResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ImageUrl,
    bool IsFirstLogin,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

