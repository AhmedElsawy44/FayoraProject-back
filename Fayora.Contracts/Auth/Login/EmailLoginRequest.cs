namespace Fayora.Contracts.Auth.Login;

public record EmailLoginRequest(
    string Email,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
