namespace Fayora.Contracts.Auth.Requests;

public record SendEmailCodeRequest(
    string Email,
     OtpPurpose Purpose
);

public enum OtpPurpose
{
    Registration,
    ResetPassword,
    AccountDeletion,
    ReactivateAccount,
}