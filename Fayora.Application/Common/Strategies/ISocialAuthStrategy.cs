using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Strategies;

public interface ISocialAuthStrategy
{
    IdentityProvider IdentityProvider { get; }
    Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken);
}
