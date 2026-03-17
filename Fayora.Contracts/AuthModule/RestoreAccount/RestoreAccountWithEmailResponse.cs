namespace Fayora.Contracts.AuthModule.RestoreAccount
{

    public record RestoreAccountWithEmailResponse(
        string UserId,
        string Email,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );

}
