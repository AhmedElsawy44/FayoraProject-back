namespace Fayora.Contracts.Auth.ResetPassword;

public record VerifyResetPhonePasswordRequest(
    string PhoneNumber,
    string Code);
