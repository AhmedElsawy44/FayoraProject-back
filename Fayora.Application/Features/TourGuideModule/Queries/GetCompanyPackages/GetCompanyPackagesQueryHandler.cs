//using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
//using Fayora.Domain.Common.Results;
//using MediatR;

//namespace Fayora.Application.Features.TourGuideModule.Queries.GetCompanyPackages
//{

//    public class GetCompanyPackagesQueryHandler(
//        ITourCompanyRepository tourCompanyRepository)
//        : IRequestHandler<GetCompanyPackagesQuery, Result<List<GetCompanyPackagesResult>>>
//    {
//        public async Task<Result<List<GetCompanyPackagesResult>>> Handle(
//            GetCompanyPackagesQuery query,
//            CancellationToken cancellationToken)
//        {

//            // Check if the tour company exists
//            var company = await tourCompanyRepository.GetTourCompanyByIdAsync(query.CompanyId, cancellationToken);

//            if (company is null)
//                return Error.NotFound(
//                    code: "TourCompany.NotFound",
//                    description: "Tour company not found.");

//            var packages = await tourCompanyRepository.GetCompanyPackagesAsync(query.CompanyId, cancellationToken);

//            return packages.Select(p => new GetCompanyPackagesResult(
//                PackageId: p.Id,
//                CompanyId: p.CompanyId,
//                Title: p.Title,
//                Description: p.Description,
//                TourTypes: (int)p.TourTypes,
//                DurationHours: p.DurationHours,
//                MaxCapacity: p.MaxCapacity,
//                AvailableSpots: p.AvailableSpots,
//                AdultPrice: p.AdultPrice,
//                ChildPrice: p.ChildPrice,
//                IsActive: p.IsActive,
//                Views: p.Views,
//                MainImageUrl: p.MainImageUrl,
//                StartDate: p.StartDate,
//                EndDate: p.EndDate,
//                CreatedAt: p.CreatedAt))
//            .ToList();
//        }
//    }
//}
