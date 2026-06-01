namespace Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;

/// <summary>
/// The result DTO returned by the housing unit recommendation query.
/// Contains the housing unit data and recommendation details.
/// </summary>
public record RecommendedUnitResult(
    Guid UnitId,
    string Title,
    decimal PricePerNight,
    string MainImageUrl,
    string AddressDetails,
    decimal Rating,
    int Views,
    double RecommendationScore,
    string RecommendationReason,
    decimal DiscountedPricePerNight = 0);
