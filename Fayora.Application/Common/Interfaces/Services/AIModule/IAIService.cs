namespace Fayora.Application.Common.Interfaces.Services.AIModule;

public interface IAIService
{
    Task<AIExtractionResult> ExtractSearchParametersAsync(string userMessage);

    Task<string> GenerateChatTitleAsync(string firstUserMessage);

    Task<string> GenerateFriendlyResponseAsync(
        string userMessage,
        string rawContextData,
        string userMetadata);

    public record AIExtractionResult(string Intent, SearchParams Params, string SearchQuery);

    public record SearchParams(string Location, decimal? Budget_Max, int? People_Count, string Trip_Provider, int? Duration_Days);
}