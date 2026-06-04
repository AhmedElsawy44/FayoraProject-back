using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetSimilarUnits;

public class GetSimilarUnitsQueryHandler(
    IRecommendationService recommendationService,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetSimilarUnitsQuery, Result<List<RecommendedUnitResult>>>
{
    public async Task<Result<List<RecommendedUnitResult>>> Handle(
        GetSimilarUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var recommendations = await recommendationService.GetSimilarUnitsAsync(request.UnitId, count, cancellationToken);

        for (int i = 0; i < recommendations.Count; i++)
        {
            var item = recommendations[i];
            decimal discountedPrice = item.PricePerNight;
            var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                item.UnitId, OfferTargetType.HousingUnit, cancellationToken);
            var offer = activeOffers.FirstOrDefault();
            if (offer is not null)
            {
                var discountResult = offer.ApplyTo(item.PricePerNight);
                if (!discountResult.IsError)
                {
                    discountedPrice = discountResult.Value;
                }
            }
            recommendations[i] = item with { DiscountedPricePerNight = discountedPrice };
        }

        return Result<List<RecommendedUnitResult>>.CreateSuccess(recommendations);
    }
}
