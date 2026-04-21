namespace Fayora.Contracts.AuthModule.RestoreAccount;


public record RestoreAccountResponse
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
