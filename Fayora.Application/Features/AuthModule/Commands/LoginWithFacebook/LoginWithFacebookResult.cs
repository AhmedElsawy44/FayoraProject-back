namespace Fayora.Application.Features.AuthModule.Commands.LoginWithFacebook;

public record LoginWithFacebookResult 
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
