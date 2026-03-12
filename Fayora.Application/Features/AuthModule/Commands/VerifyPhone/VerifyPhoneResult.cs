namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public record VerifyPhoneResult(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
