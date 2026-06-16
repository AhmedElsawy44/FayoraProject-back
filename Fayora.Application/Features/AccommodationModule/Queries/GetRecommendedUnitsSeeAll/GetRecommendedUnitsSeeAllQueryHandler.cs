using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnitsSeeAll;

public class GetRecommendedUnitsSeeAllQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetRecommendedUnitsSeeAllQuery, Result<List<RecommendedUnitResult>>>
{
    public async Task<Result<List<RecommendedUnitResult>>> Handle(
        GetRecommendedUnitsSeeAllQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var page = Math.Max(request.Page, 1);
        var size = Math.Clamp(request.Size, 1, 50);

        var recommendations = await recommendationService.GetUnitsSeeAllAsync(userId, page, size, cancellationToken);

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
