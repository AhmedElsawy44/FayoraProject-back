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

    /// <summary>
    /// Returns combined personalized recommendations (units, guides, packages) for home screen.
    /// </summary>
    Task<AllRecommendationsResult> GetAllRecommendationsAsync(
        Guid userId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns paginated and session-diverse package recommendations.
    /// </summary>
    Task<List<RecommendedPackageResult>> GetPackagesSeeAllAsync(
        Guid userId, int page, int size, CancellationToken cancellationToken);

    /// <summary>
    /// Returns paginated and session-diverse housing unit recommendations.
    /// </summary>
    Task<List<RecommendedUnitResult>> GetUnitsSeeAllAsync(
        Guid userId, int page, int size, CancellationToken cancellationToken);

    /// <summary>
    /// Returns paginated and session-diverse tour guide recommendations.
    /// </summary>
    Task<List<RecommendedGuideResult>> GetGuidesSeeAllAsync(
        Guid userId, int page, int size, CancellationToken cancellationToken);

    /// <summary>
    /// Returns similar housing units for a given stays item.
    /// </summary>
    Task<List<RecommendedUnitResult>> GetSimilarUnitsAsync(
        Guid unitId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns similar tour guides.
    /// </summary>
    Task<List<RecommendedGuideResult>> GetSimilarGuidesAsync(
        Guid guideId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Returns similar packages.
    /// </summary>
    Task<List<RecommendedPackageResult>> GetSimilarPackagesAsync(
        Guid packageId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Refreshes/retrains the Python recommender model.
    /// </summary>
    Task<PythonRefreshResult> RefreshAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Evaluates the Python model.
    /// </summary>
    Task<object> EvaluateAsync(int topN, string cutoffDate, CancellationToken cancellationToken);
}

