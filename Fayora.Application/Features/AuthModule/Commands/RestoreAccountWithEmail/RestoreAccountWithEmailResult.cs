namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;

public record RestoreAccountWithEmailResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
