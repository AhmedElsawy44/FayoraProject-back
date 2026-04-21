namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public record ConfirmChangePhoneResult
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
