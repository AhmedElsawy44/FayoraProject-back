using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;

/// <summary>
/// Orchestrates the tour guide recommendation pipeline:
/// - Authenticated user -> personalized recommendations (multi-signal scoring)
/// - Anonymous/cold-start -> trending guides
/// </summary>
public class GetRecommendedGuidesQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetRecommendedGuidesQuery, Result<List<RecommendedGuideResult>>>
{
    public async Task<Result<List<RecommendedGuideResult>>> Handle(
        GetRecommendedGuidesQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        // Clamp count to a reasonable range
        var count = Math.Clamp(request.Count, 1, 50);

        List<RecommendedGuideResult> recommendations;

        if (userId != Guid.Empty)
        {
            // Authenticated user -> full personalized scoring pipeline
            recommendations = await recommendationService.GetPersonalizedGuidesAsync(
                userId, count, cancellationToken);

            // If personalized returns too few (cold-start), fill remaining slots with trending
            if (recommendations.Count < count)
            {
                var excludeIds = recommendations.Select(r => r.UserId).ToHashSet();
                var trending = await recommendationService.GetTrendingGuidesAsync(
                    count - recommendations.Count, cancellationToken);

                recommendations.AddRange(
                    trending.Where(t => !excludeIds.Contains(t.UserId)));
            }
        }
        else
        {
            // Anonymous user -> trending only
            recommendations = await recommendationService.GetTrendingGuidesAsync(
                count, cancellationToken);
        }

        for (int i = 0; i < recommendations.Count; i++)
        {
            var item = recommendations[i];
            decimal? discountedRate = item.BaseRate;
            if (item.BaseRate.HasValue)
            {
                var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                    item.UserId, OfferTargetType.TourGuide, cancellationToken);
                var offer = activeOffers.FirstOrDefault();
                if (offer is not null)
                {
                    var discountResult = offer.ApplyTo(item.BaseRate.Value);
                    if (!discountResult.IsError)
                    {
                        discountedRate = discountResult.Value;
                    }
                }
            }
            recommendations[i] = item with { DiscountedBaseRate = discountedRate };
        }

        return recommendations;
    }
}
