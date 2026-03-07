using Fayora.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fayora.Infrastructure.Services.AuthServices.FacebookLoginService
{
    public class FacebookAuthService(
    HttpClient httpClient,
    IOptions<FacebookSettings> facebookSettings
) : IFacebookAuthService
    {
        private readonly FacebookSettings _settings = facebookSettings.Value;

        public async Task<FacebookUserInfo?> GetUserInfoAsync(
            string accessToken,
            CancellationToken ct = default)
        {
            // 1 - Check token validity from Facebook
            var verifyUrl = $"https://graph.facebook.com/debug_token" +
                            $"?input_token={accessToken}" +
                            $"&access_token={_settings.AppId}|{_settings.AppSecret}";

            var verifyResponse = await httpClient.GetAsync(verifyUrl, ct);
            if (!verifyResponse.IsSuccessStatusCode) return null;

            var verifyJson = await verifyResponse.Content.ReadAsStringAsync(ct);
            var verifyData = JsonSerializer.Deserialize<FacebookTokenValidation>(verifyJson);

            if (verifyData?.Data is null || !verifyData.Data.IsValid) return null;

            // 2 - take user info from Facebook
            var userUrl = $"https://graph.facebook.com/me" +
                          $"?fields=id,name,email,picture" +
                          $"&access_token={accessToken}";

            var userResponse = await httpClient.GetAsync(userUrl, ct);
            if (!userResponse.IsSuccessStatusCode) return null;

            var userJson = await userResponse.Content.ReadAsStringAsync(ct);
            var userData = JsonSerializer.Deserialize<FacebookUserResponse>(userJson);

            if (userData is null) return null;

            return new FacebookUserInfo(
                userData.Id,
                userData.Email,
                userData.Name,
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
}
