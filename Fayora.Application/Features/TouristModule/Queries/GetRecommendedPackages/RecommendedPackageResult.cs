namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;

/// <summary>
/// The result DTO returned by the recommendation endpoint.
/// Contains both the package data and the recommendation metadata.
/// </summary>
public record RecommendedPackageResult(
    Guid PackageId,
    string Title,
    decimal AdultPrice,
    int DurationHours,
    string MainImageUrl,
    string TourTypes,
    int Views,
    double RecommendationScore,
    string RecommendationReason,
    decimal DiscountedAdultPrice = 0);
