namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone;

public record ChangePhoneResult(
    Guid UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);