using Fayora.Application.Common.Interfaces.Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;

namespace Fayora.Infrastructure.Services.AuthServices;

public class MockAppleAuthService : IAppleAuthService
{
    public Task<AppleUserInfo?> GetUserInfoAsync(string identityToken, CancellationToken cancellationToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(identityToken))
            {
                return Task.FromResult<AppleUserInfo?>(null);
            }

            var jwtToken = handler.ReadJwtToken(identityToken);

            var subjectId = jwtToken.Subject;

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var userInfo = new AppleUserInfo(subjectId, email);

            return Task.FromResult<AppleUserInfo?>(userInfo);
        }
        catch (Exception)
        {
            return null!;
        }
    }
}