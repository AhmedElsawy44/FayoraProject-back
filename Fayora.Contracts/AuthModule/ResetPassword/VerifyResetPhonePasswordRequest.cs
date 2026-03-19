namespace Fayora.Contracts.AuthModule.ResetPassword;

public record VerifyResetPhonePasswordRequest(
    string PhoneNumber,
    string Code);
