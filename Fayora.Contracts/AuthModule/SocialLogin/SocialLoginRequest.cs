namespace Fayora.Contracts.AuthModule.AppleLogin;

public record SocialLoginRequest(
    string Token,
    string? FirstName,
    string? LastName,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage,
    string SocialProvider
);
