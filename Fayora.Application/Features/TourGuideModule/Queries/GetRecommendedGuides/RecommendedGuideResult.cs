namespace Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;

/// <summary>
/// The result DTO returned by the tour guide recommendation endpoint.
/// Contains both the guide data and the recommendation metadata.
/// </summary>
public record RecommendedGuideResult(
    Guid UserId,
    string FullName,
    string? ProfileImageUrl,
    decimal? BaseRate,
    int? YearsOfExperience,
    bool IsSuperGuide,
    decimal AverageRating,
    int ReviewCount,
    int Views,
    double RecommendationScore,
    string RecommendationReason,
    decimal? DiscountedBaseRate = null);
