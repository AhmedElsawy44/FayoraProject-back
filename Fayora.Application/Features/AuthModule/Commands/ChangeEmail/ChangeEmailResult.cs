namespace Fayora.Application.Features.AuthModule.Commands.ChangeEmail;

public record ChangeEmailResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
