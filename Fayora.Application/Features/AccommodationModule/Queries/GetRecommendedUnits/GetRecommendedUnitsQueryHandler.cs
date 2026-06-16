using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;

/// <summary>
/// Orchestrates the housing unit recommendation pipeline:
/// - Authenticated user → personalized recommendations (multi-signal scoring)
/// - Anonymous/cold-start → trending units
/// </summary>
public class GetRecommendedUnitsQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetRecommendedUnitsQuery, Result<List<RecommendedUnitResult>>>
{
    public async Task<Result<List<RecommendedUnitResult>>> Handle(
        GetRecommendedUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        // Clamp count to a reasonable range
        var count = Math.Clamp(request.Count, 1, 50);

        List<RecommendedUnitResult> recommendations;

        if (userId != Guid.Empty)
        {
            // Authenticated user → full personalized scoring pipeline
            recommendations = await recommendationService.GetPersonalizedUnitsAsync(
                userId, count, cancellationToken);

            // If personalized returns too few, fill remaining slots with trending
            if (recommendations.Count < count)
            {
                var excludeIds = recommendations.Select(r => r.UnitId).ToHashSet();
                var trending = await recommendationService.GetTrendingUnitsAsync(
                    count - recommendations.Count, cancellationToken);

                recommendations.AddRange(
                    trending.Where(t => !excludeIds.Contains(t.UnitId)));
            }
        }
        else
        {
            // Anonymous user → trending only
            recommendations = await recommendationService.GetTrendingUnitsAsync(
                count, cancellationToken);
        }

        return recommendations;
    }
}
