namespace Fayora.Contracts.Auth.Requests;

public record GoogleLoginRequest(
string AccessToken,
string DeviceId,
string FcmToken,
string DeviceLanguage
);
