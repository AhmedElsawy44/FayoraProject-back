namespace Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;

public record LoginWithPhoneResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);