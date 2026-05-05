using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Infrastructure.Services.AuthModule;

public class SocialAuthService(IEnumerable<ISocialAuthStrategy> strategies) : ISocialAuthService
{
    private readonly IReadOnlyDictionary<IdentityProvider, ISocialAuthStrategy> _strategies =
        strategies.ToDictionary(strategies => strategies.IdentityProvider);

    public async Task<SocialUserInfo?> GetUserInfoAsync(string identityToken, IdentityProvider identityProvider, CancellationToken cancellationToken)
    {
        if (!_strategies.TryGetValue(identityProvider, out var strategy))
        {
            throw new NotSupportedException($"Identity provider {identityProvider} is not supported.");
        }

        return await strategy.LoginWithSocialAsync(identityToken, cancellationToken);
    }
}
