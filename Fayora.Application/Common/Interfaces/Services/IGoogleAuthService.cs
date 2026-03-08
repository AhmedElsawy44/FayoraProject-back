namespace Fayora.Application.Common.Interfaces.Services;

public interface IGoogleAuthService
{
    Task<GoogleAuthenticationResult?> GetUserInfoAsync(string token, CancellationToken cancellationToken);

    public record GoogleAuthenticationResult(
        string Id,
        string Email,
        string FirstName,
        string LastName,
        string? PictureUrl);
}
