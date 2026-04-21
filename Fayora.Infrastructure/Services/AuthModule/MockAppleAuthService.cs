using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.IdentityModule;
using System.IdentityModel.Tokens.Jwt;

namespace Fayora.Infrastructure.Services.AuthModule;

public class MockAppleAuthService : ISocialAuthStrategy
{
    public IdentityProvider IdentityProvider => IdentityProvider.Apple;

    public Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return Task.FromResult<SocialUserInfo?>(null);
            }

            var jwtToken = handler.ReadJwtToken(token);

            var subjectId = jwtToken.Subject;

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var userInfo = new SocialUserInfo(subjectId, email, null, null, null);

            return Task.FromResult<SocialUserInfo?>(userInfo);
        }
        catch (Exception)
        {
            return null!;
        }
    }
}