namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail
{
    public record RestoreAccountWithEmailResult(
        Guid UserId,
        string Email,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );

}
