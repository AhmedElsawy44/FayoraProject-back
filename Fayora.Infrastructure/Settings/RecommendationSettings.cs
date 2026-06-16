namespace Fayora.Infrastructure.Settings;

public class RecommendationSettings
{
    public const string SectionName = "RecommendationSettings";
    public string BaseUrl { get; init; } = "http://localhost:8000";
}
