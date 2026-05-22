using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedLocations;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;

namespace Fayora.Application.Common.Interfaces.Services.RecommendationModule;

public interface IRecommendationService
{
    /// <summary>
    /// Returns personalized package recommendations for an authenticated user
    /// using content-based, collaborative, and popularity signals.
    /// </summary>
    Task<List<RecommendedPackageResult>> GetPersonalizedAsync(
        Guid userId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns trending packages for anonymous or cold-start users
    /// using popularity and recency signals with diversity injection.
    /// </summary>
    Task<List<RecommendedPackageResult>> GetTrendingAsync(
        int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns personalized unit recommendations for an authenticated user
    /// using budget and travel style matching, collaborative filtering, popularity, and recency signals.
    /// </summary>
    Task<List<RecommendedUnitResult>> GetPersonalizedUnitsAsync(
        Guid userId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns trending units for anonymous or cold-start users
    /// using popularity and recency signals.
    /// </summary>
    Task<List<RecommendedUnitResult>> GetTrendingUnitsAsync(
        int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns personalized tour guide recommendations for an authenticated user
    /// using content-based, collaborative, popularity, and recency signals.
    /// </summary>
    Task<List<RecommendedGuideResult>> GetPersonalizedGuidesAsync(
        Guid userId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns trending tour guides for anonymous or cold-start users
    /// using popularity and recency signals.
    /// </summary>
    Task<List<RecommendedGuideResult>> GetTrendingGuidesAsync(
        int count, CancellationToken cancellationToken);
}


    /// <summary>
    /// Returns personalized location recommendations for an authenticated user.
    /// Scores locations by aggregating their associated packages' recommendation scores.
    /// </summary>
    Task<List<RecommendedLocationResult>> GetPersonalizedLocationsAsync(
        Guid userId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns trending locations for anonymous or cold-start users.
    /// Scores locations by aggregating their associated packages' trending scores.
    /// </summary>
    Task<List<RecommendedLocationResult>> GetTrendingLocationsAsync(
        int count, CancellationToken cancellationToken);
}
