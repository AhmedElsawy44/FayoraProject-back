namespace Fayora.Contracts.AuthModule.RestoreAccount;

public record RestoreAccountRequest
(
    string Code,
    string Identifier,
    string CodeDeliveryMethod,
    string FcmToken,
    string DeviceLanguage
);
