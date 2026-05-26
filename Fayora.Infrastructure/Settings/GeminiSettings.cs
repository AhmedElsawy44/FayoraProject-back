namespace Fayora.Infrastructure.Settings;

public class GeminiSettings
{
    public const string SectionName = "GeminiSettings";
    public string ApiKey { get; init; } = null!;
    public System.Collections.Generic.List<string> ApiKeys { get; init; } = new();
    public string Model { get; init; } = "gemini-2.5-flash";
    public string BotUserId { get; init; } = "00000000-0000-0000-0000-000000000001";
}

