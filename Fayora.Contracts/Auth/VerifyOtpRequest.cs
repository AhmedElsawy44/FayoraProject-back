namespace Fayora.Contracts.Auth;

public record VerifyOtpRequest(Guid UserId, string? Email, string? Phone, string Code, VerifyOtpDeviceInfoDto DeviceInfoDto);

public record VerifyOtpDeviceInfoDto(
    string DeviceId,
    string FcmToken);

