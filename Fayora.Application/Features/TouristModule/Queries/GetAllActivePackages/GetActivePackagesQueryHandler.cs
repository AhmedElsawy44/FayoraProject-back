using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{

    public class GetActivePackagesQueryHandler(
        IPackageRepository packageRepository,
        IDiscountOfferRepository discountOfferRepository,
        IUserRepository userRepository)
        : IQueryHandler<GetActivePackagesQuery, Result<GetActivePackagesResult>>
    {
        public async Task<Result<GetActivePackagesResult>> Handle(
            GetActivePackagesQuery request,
            CancellationToken cancellationToken)
        {


            var (items, totalCount) = await packageRepository.GetActivePackagesAsync(
                request.Search,
                request.LocationId,
                request.ProviderType,
                null,
                request.MinDuration,
                request.MaxDuration,
                request.MinPrice,
                request.MaxPrice,
                request.Page,
                request.PageSize,
                cancellationToken);

            var userIds = items.Select(p => p.UserId).Distinct().ToList();
            var users = await userRepository.GetUsersByIdsAsync(
                userIds,
                new IUserRepository.UserQueryOptions(IsReadOnly: true),
                cancellationToken);
            var userMap = users.ToDictionary(u => u.Id);

            var summaryItems = items.Select(p =>
            {
                userMap.TryGetValue(p.UserId, out var u);
                return new ActivePackageSummaryResult(
                    p.Id,
                    p.Title,
                    p.AdultPrice,
                    p.AdultPrice,
                    p.DurationHours,
                    p.MainImageUrl?.Value ?? "",
                    p.TourTypes.ToString(),
                    p.AverageRating,
                    p.Views,
                    u != null ? (u.FirstName + " " + u.LastName) : null,
                    (u != null && u.ProfileImageUrl != null) ? u.ProfileImageUrl.Value : null,
                    (int)p.ProviderType
                );
            }).ToList();


            for (int i = 0; i < summaryItems.Count; i++)
            {
                var item = summaryItems[i];
                decimal discountedPrice = item.AdultPrice;
                var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                    item.Id, OfferTargetType.GuidePackage, cancellationToken);
                var offer = activeOffers.FirstOrDefault();
                if (offer is not null)
                {
                    var discountResult = offer.ApplyTo(item.AdultPrice);
                    if (!discountResult.IsError)
                    {
                        discountedPrice = discountResult.Value;
                    }
                }
                summaryItems[i] = item with { DiscountedAdultPrice = discountedPrice };
            }

            var result = new GetActivePackagesResult(
                summaryItems,
                totalCount,
                request.Page,
                request.PageSize);

            return result;
        }
    }
}
