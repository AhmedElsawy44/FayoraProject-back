namespace Fayora.Contracts.Auth;

public record VerifyRegisterOtpRequestDto(Guid UserId, string? Email, string? PhoneNumber, string? SimCountryIsoCode, string Otp, VerifyOtpDeviceInfoDto DeviceInfoDto);

public record VerifyOtpDeviceInfoDto(
    string DeviceId,
    string FcmToken);

