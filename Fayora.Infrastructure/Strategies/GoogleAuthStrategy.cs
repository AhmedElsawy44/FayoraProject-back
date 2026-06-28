using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fayora.Infrastructure.Strategies;

public class GoogleAuthStrategy(
    HttpClient httpClient,
    IOptions<GoogleSettings> settings) : ISocialAuthStrategy
{
    private readonly GoogleSettings _settings = settings.Value;

    public IdentityProvider IdentityProvider => IdentityProvider.Google;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            if (IsJwtToken(token))
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = _settings.ClientIds
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
            else
            {
                // Validate using Google UserInfo API (for Access Tokens)
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={token}", cancellationToken);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var payload = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json);

                if (payload is null) return null;

                return new SocialUserInfo(
                    SubjectId: payload.Sub,
                    Email: payload.Email,
                    FirstName: payload.GivenName,
                    LastName: payload.FamilyName,
                    PictureUrl: payload.Picture
                );
            }
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

    private static bool IsJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var parts = token.Split('.');
        return parts.Length == 3;
    }
}

// Helper Class
file record GoogleUserInfoResponse(
    [property: JsonPropertyName("sub")] string Sub,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("given_name")] string? GivenName,
    [property: JsonPropertyName("family_name")] string? FamilyName,
    [property: JsonPropertyName("picture")] string? Picture
);