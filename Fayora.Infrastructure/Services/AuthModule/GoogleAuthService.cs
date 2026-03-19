using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Infrastructure.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using static Fayora.Application.Common.Interfaces.Services.AuthModule.IGoogleAuthService;

namespace Fayora.Infrastructure.Services.AuthModule;

public class GoogleAuthService(
    IOptions<GoogleSettings> settings) : IGoogleAuthService
{
    private readonly GoogleSettings _settings = settings.Value;
    public async Task<GoogleAuthenticationResult?> GetUserInfoAsync(
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_settings.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            if (payload is null) return null;

            return new IGoogleAuthService.GoogleAuthenticationResult(
                Id: payload.Subject,
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