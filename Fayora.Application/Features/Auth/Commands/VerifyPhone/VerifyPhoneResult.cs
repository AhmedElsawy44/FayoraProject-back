namespace Fayora.Application.Features.Auth.Commands.VerifyPhone;

public record VerifyPhoneResult(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
