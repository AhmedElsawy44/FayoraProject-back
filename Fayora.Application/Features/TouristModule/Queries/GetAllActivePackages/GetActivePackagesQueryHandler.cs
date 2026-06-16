using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{

    public class GetActivePackagesQueryHandler(
        IPackageRepository packageRepository)
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

            var result = new GetActivePackagesResult(
                items.Select(p => new ActivePackageSummaryResult(
                    p.Id,
                    p.Title,
                    p.AdultPrice,
                    p.DurationHours,
                    p.MainImageUrl?.Value ?? "",
                    p.TourTypes.ToString(),
                    0m,
                    p.Views)).ToList(),
                totalCount,
                request.Page,
                request.PageSize);

            return result;
        }
    }
}
