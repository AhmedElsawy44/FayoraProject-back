using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;

/// <summary>
/// Orchestrates the recommendation pipeline:
/// - Authenticated user → personalized recommendations (multi-signal scoring)
/// - Anonymous/cold-start → trending packages with diversity
/// </summary>
public class GetRecommendedPackagesQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetRecommendedPackagesQuery, Result<List<RecommendedPackageResult>>>
{
    public async Task<Result<List<RecommendedPackageResult>>> Handle(
        GetRecommendedPackagesQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        // Clamp count to a reasonable range
        var count = Math.Clamp(request.Count, 1, 50);

        List<RecommendedPackageResult> recommendations;

        if (userId != Guid.Empty)
        {
            // Authenticated user → full personalized scoring pipeline
            recommendations = await recommendationService.GetPersonalizedAsync(
                userId, count, cancellationToken);

            // If personalized returns too few (cold-start: new user, no interactions),
            // fill remaining slots with trending
            if (recommendations.Count < count)
            {
                var excludeIds = recommendations.Select(r => r.PackageId).ToHashSet();
                var trending = await recommendationService.GetTrendingAsync(
                    count - recommendations.Count, cancellationToken);

                recommendations.AddRange(
                    trending.Where(t => !excludeIds.Contains(t.PackageId)));
            }
        }
        else
        {
            // Anonymous user → trending only
            recommendations = await recommendationService.GetTrendingAsync(
                count, cancellationToken);
        }

        for (int i = 0; i < recommendations.Count; i++)
        {
            var item = recommendations[i];
            decimal discountedPrice = item.AdultPrice;
            var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                item.PackageId, OfferTargetType.GuidePackage, cancellationToken);
            var offer = activeOffers.FirstOrDefault();
            if (offer is not null)
            {
                var discountResult = offer.ApplyTo(item.AdultPrice);
                if (!discountResult.IsError)
                {
                    discountedPrice = discountResult.Value;
                }
            }
            recommendations[i] = item with { DiscountedAdultPrice = discountedPrice };
        }

        return recommendations;
    }
}
