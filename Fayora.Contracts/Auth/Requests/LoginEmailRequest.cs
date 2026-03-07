namespace Fayora.Contracts.Auth.Requests;

public record LoginEmailRequest(
    string Email,
    string Password,
    string FcmToken,
    string DeviceLanguage
);
