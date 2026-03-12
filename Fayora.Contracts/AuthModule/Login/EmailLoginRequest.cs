namespace Fayora.Contracts.AuthModule.Login;

public record EmailLoginRequest(
    string Email,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
