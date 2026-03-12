namespace Fayora.Contracts.AuthModule.ResetPassword;

public record ResetPhonePasswordRequest(
    string PhoneNumber,
    string ResetToken,
    string NewPassword
);