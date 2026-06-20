using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public record AppleUserInfo(string SubjectId, string? Email);
public interface ISocialAuthService
{

    Task<SocialUserInfo?> GetUserInfoAsync(string identityToken, IdentityProvider identityProvider, CancellationToken cancellationToken);
}

public record SocialUserInfo(
    string SubjectId,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PictureUrl,
    string? PhoneNumber = null
);
