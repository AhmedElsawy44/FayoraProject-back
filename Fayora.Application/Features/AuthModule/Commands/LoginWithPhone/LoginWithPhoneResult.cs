namespace Fayora.Application.Features.Auth.Commands.LoginWithPhone;

public record LoginWithPhoneResult
(
    Guid UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);