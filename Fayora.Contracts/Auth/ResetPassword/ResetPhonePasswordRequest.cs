namespace Fayora.Contracts.Auth.ResetPassword;

public record ResetPhonePasswordRequest(
    string PhoneNumber,
    string ResetToken,
    string NewPassword
);