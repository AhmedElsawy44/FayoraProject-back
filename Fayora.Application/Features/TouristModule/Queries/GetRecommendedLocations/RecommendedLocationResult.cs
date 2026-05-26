namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedLocations;

/// <summary>
/// The result DTO returned by the location recommendation endpoint.
/// Contains both the location data and the recommendation metadata.
/// </summary>
public record RecommendedLocationResult(
    int LocationId,
    string Name,
    string MainImageUrl,
    decimal Rating,
    string Category,
    int PackageCount,
    double RecommendationScore,
    string RecommendationReason);
