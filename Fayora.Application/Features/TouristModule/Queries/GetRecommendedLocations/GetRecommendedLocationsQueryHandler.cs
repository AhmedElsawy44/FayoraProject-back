using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedLocations;

/// <summary>
/// Orchestrates the location recommendation pipeline:
/// - Authenticated user → personalized location recommendations (derived from package scoring)
/// - Anonymous/cold-start → trending locations with diversity
/// </summary>
public class GetRecommendedLocationsQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetRecommendedLocationsQuery, Result<List<RecommendedLocationResult>>>
{
    public async Task<Result<List<RecommendedLocationResult>>> Handle(
        GetRecommendedLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        // Clamp count to a reasonable range
        var count = Math.Clamp(request.Count, 1, 50);

        List<RecommendedLocationResult> recommendations;

        if (userId != Guid.Empty)
        {
            // Authenticated user → full personalized scoring pipeline
            recommendations = await recommendationService.GetPersonalizedLocationsAsync(
                userId, count, cancellationToken);

            // If personalized returns too few (cold-start: new user, no interactions),
            // fill remaining slots with trending
            if (recommendations.Count < count)
            {
                var excludeIds = recommendations.Select(r => r.LocationId).ToHashSet();
                var trending = await recommendationService.GetTrendingLocationsAsync(
                    count - recommendations.Count, cancellationToken);

                recommendations.AddRange(
                    trending.Where(t => !excludeIds.Contains(t.LocationId)));
            }
        }
        else
        {
            // Anonymous user → trending only
            recommendations = await recommendationService.GetTrendingLocationsAsync(
                count, cancellationToken);
        }

        return recommendations;
    }
}
