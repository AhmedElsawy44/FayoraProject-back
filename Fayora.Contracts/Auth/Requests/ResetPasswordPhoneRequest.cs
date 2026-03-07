namespace Fayora.Contracts.Auth.Requests;

public record ResetPasswordPhoneRequest(
    string PhoneNumber,
    string ResetToken,
    string NewPassword
);