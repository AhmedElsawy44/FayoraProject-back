namespace Fayora.Contracts.Auth.Requests;

public record EmailLoginRequest(
    string Email,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
