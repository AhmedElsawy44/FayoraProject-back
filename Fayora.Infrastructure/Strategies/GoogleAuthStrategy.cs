using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Strategies;

public class GoogleAuthStrategy(
    IOptions<GoogleSettings> settings) : ISocialAuthStrategy
{
    private readonly GoogleSettings _settings = settings.Value;

    public IdentityProvider IdentityProvider => IdentityProvider.Google;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_settings.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            if (payload is null) return null;

            return new SocialUserInfo(
                SubjectId: payload.Subject,
                Email: payload.Email,
                FirstName: payload.GivenName,
                LastName: payload.FamilyName,
                PictureUrl: payload.Picture
            );
        }
        catch (InvalidJwtException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}