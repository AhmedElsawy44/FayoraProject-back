namespace Fayora.Contracts.AuthModule.RestoreAccount;

public record RestoreAccountWithPhoneRequest(
    string PhoneNumber,
    string Code,
    string FcmToken,
    string DeviceLanguage
);
