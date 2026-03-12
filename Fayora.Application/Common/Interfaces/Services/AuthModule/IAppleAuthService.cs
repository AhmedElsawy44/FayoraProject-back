namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public record AppleUserInfo(string SubjectId, string? Email);
public interface IAppleAuthService
{
    Task<AppleUserInfo?> GetUserInfoAsync(string identityToken, CancellationToken cancellationToken);
}
