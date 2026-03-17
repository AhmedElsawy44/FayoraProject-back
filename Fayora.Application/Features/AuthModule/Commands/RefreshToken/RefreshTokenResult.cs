namespace Fayora.Application.Features.AuthModule.Commands.RefreshToken
{
    public record RefreshTokenResult(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
