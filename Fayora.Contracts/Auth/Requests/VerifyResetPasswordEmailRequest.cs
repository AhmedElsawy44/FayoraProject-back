namespace Fayora.Contracts.Auth.Requests;

public record VerifyResetPasswordEmailRequest(
    string Email,
    string Code);
