namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public record LoginWithEmailResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);