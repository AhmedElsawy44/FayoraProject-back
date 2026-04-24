using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fayora.Infrastructure.Strategies;

public class FacebookAuthStrategy(
HttpClient httpClient,
IOptions<FacebookSettings> facebookSettings
) : ISocialAuthStrategy
{
    private readonly FacebookSettings _settings = facebookSettings.Value;

    public IdentityProvider IdentityProvider => IdentityProvider.Facebook;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken ct)
    {
        // 1 - Check token validity from Facebook
        var verifyUrl = $"https://graph.facebook.com/debug_token" +
                        $"?input_token={token}" +
                        $"&access_token={_settings.AppId}|{_settings.AppSecret}";

        var verifyResponse = await httpClient.GetAsync(verifyUrl, ct);
        if (!verifyResponse.IsSuccessStatusCode) return null;

        var verifyJson = await verifyResponse.Content.ReadAsStringAsync(ct);
        var verifyData = JsonSerializer.Deserialize<FacebookTokenValidation>(verifyJson);

        if (verifyData?.Data is null
            || !verifyData.Data.IsValid
            || verifyData.Data.AppId != _settings.AppId) return null;

        // 2 - take user info from Facebook
        var userUrl = $"https://graph.facebook.com/me" +
                      $"?fields=id,name,email,picture" +
                      $"&access_token={token}";

        var userResponse = await httpClient.GetAsync(userUrl, ct);
        if (!userResponse.IsSuccessStatusCode) return null;

        var userJson = await userResponse.Content.ReadAsStringAsync(ct);
        var userData = JsonSerializer.Deserialize<FacebookUserResponse>(userJson);

        if (userData is null) return null;

        var nameParts = userData.Name?.Split(" ");
        var firstName = nameParts?.FirstOrDefault();
        var lastName = nameParts?.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : null;

        return new SocialUserInfo(
            userData.Id,
            userData.Email,
            firstName,
            lastName,
            userData.Picture?.Data?.Url
        );
    }
}

// Helper Classes
file record FacebookTokenValidation(
    [property: JsonPropertyName("data")] FacebookTokenData? Data
);

file record FacebookTokenData(
    [property: JsonPropertyName("is_valid")] bool IsValid,
    [property: JsonPropertyName("app_id")] string? AppId
);

file record FacebookUserResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("picture")] FacebookPicture? Picture
);

file record FacebookPicture(
    [property: JsonPropertyName("data")] FacebookPictureData? Data
);

file record FacebookPictureData(
    [property: JsonPropertyName("url")] string? Url
);
