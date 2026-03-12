namespace Fayora.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenResult(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
