namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone
{
    public record RestoreAccountWithPhoneResult(
        Guid UserId,
        string PhoneNumber,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
