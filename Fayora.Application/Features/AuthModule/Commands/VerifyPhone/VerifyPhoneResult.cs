namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public record VerifyPhoneResult
(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
