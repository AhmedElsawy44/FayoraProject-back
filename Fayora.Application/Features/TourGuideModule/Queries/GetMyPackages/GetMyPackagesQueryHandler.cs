using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages.Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages
{

    public class GetMyPackagesQueryHandler(
        IPackageRepository packageRepository,
        IClientContextProvider clientContextProvider)
        : IQueryHandler<GetMyPackagesQuery, Result<GetMyPackagesResult>>
    {
        public async Task<Result<GetMyPackagesResult>> Handle(
            GetMyPackagesQuery request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var (items, totalCount) = await packageRepository.GetMyPackagesAsync(
                userId,
                request.Status,
                request.Page,
                request.PageSize,
                cancellationToken);

            var result = new GetMyPackagesResult(
                items.Select(p => new PackageSummaryResult(
                    p.Id,
                    p.Title,
                    p.AdultPrice,
                    p.ChildPrice,
                    p.DurationHours,
                    p.MaxCapacity,
                    p.MainImageUrl.Value,
                    p.TourTypes.ToString(),
                    p.PackageStatus.ToString())).ToList(),
                totalCount,
                request.Page,
                request.PageSize);

            return result;
        }
    }
}
