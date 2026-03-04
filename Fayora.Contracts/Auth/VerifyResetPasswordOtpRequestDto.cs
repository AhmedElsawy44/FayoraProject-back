namespace Fayora.Contracts.Auth;

public record VerifyResetPasswordOtpRequestDto(
    string? Email,
    string? PhoneNumber,
    string? SimCountryIsoCode,
    string Otp
);