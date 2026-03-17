namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public record AppleUserInfo(string SubjectId, string? Email);
public interface IAppleAuthService
{
    Task<AppleUserInfo?> GetUserInfoAsync(string identityToken, CancellationToken cancellationToken);
}
