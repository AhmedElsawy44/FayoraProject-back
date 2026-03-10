namespace Fayora.Contracts.Auth.RestoreAccount
{
    public record RestoreAccountWithPhoneRequest(
        string PhoneNumber,
        string Code,
        string FcmToken,
        string DeviceLanguage
    );
}
