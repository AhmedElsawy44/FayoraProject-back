using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Queries.GetAllPackages
{
    public class GetAllPackagesQueryHandler(
        ITourCompanyRepository tourCompanyRepository)
        : IRequestHandler<GetAllPackagesQuery, Result<GetAllPackagesResult>>
    {
        public async Task<Result<GetAllPackagesResult>> Handle(
            GetAllPackagesQuery query,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await tourCompanyRepository.GetAllPackagesAsync(
                query.TourType,
                query.MinPrice,
                query.MaxPrice,
                query.StartDate,
                query.EndDate,
                query.Page,
                query.PageSize,
                cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            return new GetAllPackagesResult(
                Items: items.Select(p => new PackageItemResult(
                    PackageId: p.Id,
                    CompanyId: p.CompanyId,
                    CompanyName: p.Company.CompanyName,
                    Title: p.Title,
                    Description: p.Description,
                    TourTypes: (int)p.TourTypes,
                    DurationHours: p.DurationHours,
                    MaxCapacity: p.MaxCapacity,
                    AvailableSpots: p.AvailableSpots,
                    AdultPrice: p.AdultPrice,
                    ChildPrice: p.ChildPrice,
                    MainImageUrl: p.MainImageUrl,
                    StartDate: p.StartDate,
                    EndDate: p.EndDate))
                .ToList(),
                TotalCount: totalCount,
                Page: query.Page,
                PageSize: query.PageSize,
                TotalPages: totalPages);
        }
    }
}
