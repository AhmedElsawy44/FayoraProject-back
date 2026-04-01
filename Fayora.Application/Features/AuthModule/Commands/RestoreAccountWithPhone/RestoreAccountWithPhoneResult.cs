namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone;

public record RestoreAccountWithPhoneResult(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
