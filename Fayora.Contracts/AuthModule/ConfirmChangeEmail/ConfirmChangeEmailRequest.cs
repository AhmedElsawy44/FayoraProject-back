namespace Fayora.Contracts.AuthModule.ConfirmChangeEmail;

public record ConfirmChangeEmailRequest
(
    string NewEmail,
    string Code
);
