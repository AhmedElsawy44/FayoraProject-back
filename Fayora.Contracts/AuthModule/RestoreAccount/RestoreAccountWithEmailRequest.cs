namespace Fayora.Contracts.AuthModule.RestoreAccount
{
    public record RestoreAccountWithEmailRequest(
        string Email,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    );
}
