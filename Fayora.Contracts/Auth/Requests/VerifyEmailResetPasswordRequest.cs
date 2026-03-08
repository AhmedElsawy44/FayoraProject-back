namespace Fayora.Contracts.Auth.Requests;

public record VerifyEmailResetPasswordRequest(
    string Email,
    string Code);
