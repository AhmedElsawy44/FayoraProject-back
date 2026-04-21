namespace Fayora.Application.Features.AuthModule.Commands.LoginWithSocial;

public record LoginWithSocialResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    bool IsFirstLogin,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);