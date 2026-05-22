namespace Fayora.Application.Features.AuthModule.Commands.VerifyAdminLogin;

public record VerifyAdminLoginResult
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
