using Fayora.Application.Common.Interfaces.Services.AIModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
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

    public async Task<string> GenerateFriendlyResponseAsync(string userMessage, string rawContextData, string userMetadata)
    {
        var activePrompt = aiSettings.Value.Step2_Sales_Prompt;

        var fullMessage = $"User Name/Context: {userMetadata}\nUser Asked: {userMessage}\nFound Data: {rawContextData}";

        var payload = CreatePayload("openai/gpt-4o-mini", activePrompt, fullMessage);
        return await SendRequestAsync(payload);
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