namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailResult
(
    Guid Id,
    string Email,
    string AccessToken,
    int ExpiresIn
);