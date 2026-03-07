namespace Fayora.Contracts.Auth.Requests;

public record VerifyResetPasswordPhoneRequest(
string PhoneNumber,
string DeviceId,
string Code);
