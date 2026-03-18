namespace Fayora.Contracts.AuthModule.ChangeEmail;

public record ChangeEmailRequest(
    string Email,
    string Password
);
