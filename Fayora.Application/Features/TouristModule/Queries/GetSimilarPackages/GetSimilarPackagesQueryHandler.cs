using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetSimilarPackages;

public class GetSimilarPackagesQueryHandler(
    IRecommendationService recommendationService,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetSimilarPackagesQuery, Result<List<RecommendedPackageResult>>>
{
    public async Task<Result<List<RecommendedPackageResult>>> Handle(
        GetSimilarPackagesQuery request,
        CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var recommendations = await recommendationService.GetSimilarPackagesAsync(request.PackageId, count, cancellationToken);

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

        return Result<List<RecommendedPackageResult>>.CreateSuccess(recommendations);
    }
}
