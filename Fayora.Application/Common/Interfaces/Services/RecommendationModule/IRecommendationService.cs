using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;

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
}
