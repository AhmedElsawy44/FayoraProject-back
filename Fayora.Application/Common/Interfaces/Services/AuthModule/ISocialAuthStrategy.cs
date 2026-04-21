using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface ISocialAuthStrategy
{
    IdentityProvider IdentityProvider { get; }
    Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken);
}
