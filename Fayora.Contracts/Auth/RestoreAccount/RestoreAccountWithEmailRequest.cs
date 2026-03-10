namespace Fayora.Contracts.Auth.RestoreAccount
{
    public record RestoreAccountWithEmailRequest(
        string Email,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    );
}
