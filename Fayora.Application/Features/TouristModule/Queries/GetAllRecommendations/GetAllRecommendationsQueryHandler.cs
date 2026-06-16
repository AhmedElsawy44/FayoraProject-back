using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllRecommendations;

public class GetAllRecommendationsQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetAllRecommendationsQuery, Result<AllRecommendationsResult>>
{
    public async Task<Result<AllRecommendationsResult>> Handle(
        GetAllRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var count = Math.Clamp(request.Count, 1, 20);

        var result = await recommendationService.GetAllRecommendationsAsync(userId, count, cancellationToken);

        // Apply discount offers for Housing Units
        for (int i = 0; i < result.Housing.Count; i++)
        {
            var item = result.Housing[i];
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
            result.Housing[i] = item with { DiscountedPricePerNight = discountedPrice };
        }

        // Apply discount offers for Packages
        for (int i = 0; i < result.Packages.Count; i++)
        {
            var item = result.Packages[i];
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
            result.Packages[i] = item with { DiscountedAdultPrice = discountedPrice };
        }

        return Result<AllRecommendationsResult>.CreateSuccess(result);
    }
}
