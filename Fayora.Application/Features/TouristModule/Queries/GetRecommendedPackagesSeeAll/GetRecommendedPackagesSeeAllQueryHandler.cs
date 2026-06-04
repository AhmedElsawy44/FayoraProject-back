using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackagesSeeAll;

public class GetRecommendedPackagesSeeAllQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider,
    IDiscountOfferRepository discountOfferRepository)
    : IQueryHandler<GetRecommendedPackagesSeeAllQuery, Result<List<RecommendedPackageResult>>>
{
    public async Task<Result<List<RecommendedPackageResult>>> Handle(
        GetRecommendedPackagesSeeAllQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var page = Math.Max(request.Page, 1);
        var size = Math.Clamp(request.Size, 1, 50);

        var recommendations = await recommendationService.GetPackagesSeeAllAsync(userId, page, size, cancellationToken);

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
