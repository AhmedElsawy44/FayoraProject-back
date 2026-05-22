namespace Fayora.Contracts.AuthModule.Verify;

public record VerifyAdminLoginRequest(
    string Email,
    string Code,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
);
