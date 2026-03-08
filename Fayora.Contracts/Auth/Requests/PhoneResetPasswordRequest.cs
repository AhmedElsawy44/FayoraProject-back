namespace Fayora.Contracts.Auth.Requests;

public record PhoneResetPasswordRequest(
    string PhoneNumber,
    string ResetToken,
    string NewPassword
);