using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fayora.Infrastructure.Strategies;

public class GoogleAuthStrategy(
    HttpClient httpClient,
    IOptions<GoogleSettings> settings,
    ILogger<GoogleAuthStrategy> logger) : ISocialAuthStrategy
{
    private readonly GoogleSettings _settings = settings.Value;

    public IdentityProvider IdentityProvider => IdentityProvider.Google;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        logger.LogInformation("GoogleAuthStrategy: LoginWithSocialAsync called. Token length: {Length}. Prefix: {Prefix}",
            token?.Length ?? 0, token?.Length > 10 ? token[..10] : token);

        try
        {
            if (IsJwtToken(token))
            {
                logger.LogInformation("GoogleAuthStrategy: Token detected as JWT ID Token.");
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = _settings.ClientIds
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                if (payload is null)
                {
                    logger.LogWarning("GoogleAuthStrategy: JWT validation returned null payload.");
                    return null;
                }

                logger.LogInformation("GoogleAuthStrategy: JWT validation succeeded for subject {Subject}.", payload.Subject);

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
                logger.LogInformation("GoogleAuthStrategy: Token detected as Access Token. Validating via Google UserInfo API...");
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={token}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogError("GoogleAuthStrategy: Google UserInfo API returned error status {StatusCode}. Body: {Body}", response.StatusCode, errBody);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation("GoogleAuthStrategy: UserInfo API response: {Body}", json);

                var payload = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json);

                if (payload is null)
                {
                    logger.LogError("GoogleAuthStrategy: Deserialization of UserInfo response returned null.");
                    return null;
                }

                return new SocialUserInfo(
                    SubjectId: payload.Sub,
                    Email: payload.Email,
                    FirstName: payload.GivenName,
                    LastName: payload.FamilyName,
                    PictureUrl: payload.Picture
                );
            }
        }
        catch (InvalidJwtException ex)
        {
            logger.LogError(ex, "GoogleAuthStrategy: InvalidJwtException occurred.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GoogleAuthStrategy: Unexpected exception occurred during validation.");
            return null;
        }
    }

    private static bool IsJwtToken(string? token)
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