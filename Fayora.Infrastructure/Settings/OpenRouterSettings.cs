namespace Fayora.Infrastructure.Settings;

public class OpenRouterSettings
{
    public const string SectionName = "OpenRouterSettings";
    public string ApiKey { get; init; } = null!;
    public string GeminiModel { get; init; } = "google/gemini-2.5-flash";
    public string OpenAIModel { get; init; } = "openai/gpt-4o-mini";
}
