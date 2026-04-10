using Fayora.Application.Common.Interfaces.Services.AIModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Fayora.Application.Common.Interfaces.Services.AIModule.IAIService;

namespace Fayora.Infrastructure.Services.AIModule;

public class AIService(HttpClient httpClient, IOptions<AISettings> aiSettings, IOptions<OpenRouterSettings> openRouterSettings) : IAIService
{
    private const string OpenRouterUrl = "https://openrouter.ai/api/v1/chat/completions";

    public async Task<AIExtractionResult> ExtractSearchParametersAsync(string userMessage)
    {
        var activePrompt = aiSettings.Value.Step1_Extractor_Prompt;

        var payload = CreatePayload("deepseek/deepseek-chat", activePrompt, userMessage);
        var response = await SendRequestAsync(payload);

        var cleanJson = ExtractJson(response);

        return JsonSerializer.Deserialize<AIExtractionResult>(cleanJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async IAsyncEnumerable<string> GenerateFriendlyResponseAsync(
    string userMessage,
    string rawContextData,
    string userMetadata,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var activePrompt = aiSettings.Value.Step2_Sales_Prompt;
        var fullMessage = $"User Name/Context: {userMetadata}\nUser Asked: {userMessage}\nFound Data: {rawContextData}";

        var payload = new
        {
            model = "openai/gpt-4o-mini",
            messages = new[] {
            new { role = "system", content = activePrompt },
            new { role = "user", content = fullMessage }
        },
            stream = true
        };

        var request = new HttpRequestMessage(HttpMethod.Post, OpenRouterUrl);
        request.Headers.Add("Authorization", $"Bearer {openRouterSettings.Value.ApiKey}");
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6).Trim();
                if (data == "[DONE]") break;

                JsonDocument doc;
                try { doc = JsonDocument.Parse(data); } catch { continue; }

                using (doc)
                {
                    if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                        choices[0].TryGetProperty("delta", out var delta) &&
                        delta.TryGetProperty("content", out var content))
                    {
                        yield return content.GetString() ?? "";
                    }
                }
            }
        }
    }

    public async Task<string> GenerateChatTitleAsync(string firstUserMessage)
    {
        var titlePrompt = aiSettings.Value.ChatTitle_Prompt;
        var payload = CreatePayload("openai/gpt-4o-mini", titlePrompt, firstUserMessage);
        return await SendRequestAsync(payload);
    }

    private object CreatePayload(string model, string systemPrompt, string userContent)
    {
        return new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userContent }
            },
            temperature = 0.5
        };
    }

    private async Task<string> SendRequestAsync(object payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, OpenRouterUrl);
        request.Headers.Add("Authorization", $"Bearer {openRouterSettings.Value.ApiKey}");

        var jsonPayload = JsonSerializer.Serialize(payload);
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }

    private string ExtractJson(string input)
    {
        var match = Regex.Match(input, @"\{.*\}", RegexOptions.Singleline);
        return match.Success ? match.Value : input;
    }
}