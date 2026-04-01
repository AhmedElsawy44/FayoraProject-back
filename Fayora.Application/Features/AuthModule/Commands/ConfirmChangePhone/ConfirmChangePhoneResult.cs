namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public record ConfirmChangePhoneResult
(
    Guid Id,
    string Email,
    string AccessToken,
    int ExpiresIn
);
