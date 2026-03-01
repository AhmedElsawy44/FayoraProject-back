namespace Fayora.Contracts.Auth;

public record VerifyOtpRequest(string Identifier, string Code, IdentifierType Type, VerifyOtpDeviceInfoDto DeviceInfoDto);

public record VerifyOtpDeviceInfoDto(
    string DeviceId,
    string FcmToken);

