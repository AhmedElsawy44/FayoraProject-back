namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);