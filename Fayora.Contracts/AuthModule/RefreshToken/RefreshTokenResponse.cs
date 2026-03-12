namespace Fayora.Contracts.AuthModule.RefreshToken
{
    public record RefreshTokenResponse(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
