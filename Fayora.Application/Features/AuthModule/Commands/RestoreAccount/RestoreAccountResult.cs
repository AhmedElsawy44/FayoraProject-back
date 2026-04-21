namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;

public record RestoreAccountResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Identifier,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
