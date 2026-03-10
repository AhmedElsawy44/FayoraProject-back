namespace Fayora.Contracts.Auth.RefreshToken
{
    public record RefreshTokenResponse(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
