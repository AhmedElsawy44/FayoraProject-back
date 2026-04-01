namespace Fayora.Application.Features.AuthModule.Commands.LoginWithApple;

public record LoginWithAppleResult
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