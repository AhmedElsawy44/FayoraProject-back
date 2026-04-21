namespace Fayora.Application.Features.AuthModule.Commands.VerifyEmail;

public record VerifyEmailResult
(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
