using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetLocationDetails
{
    public class GetLocationDetailsQueryHandler(
        ILocationRepository locationRepository,
        ILocationImageRepository locationImageRepository,
        IPackageRepository packageRepository)
        : IQueryHandler<GetLocationDetailsQuery, Result<GetLocationDetailsResult>>
    {
        public async Task<Result<GetLocationDetailsResult>> Handle(
            GetLocationDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var location = await locationRepository.GetLocationByIdAsync(
                request.LocationId, cancellationToken);
            if (location is null)
                return Error.NotFound("Location.NotFound", "Location not found.");

            var (packages, _) = await packageRepository.GetActivePackagesAsync(
                search: null,
                locationId: request.LocationId,
                providerType: null,
                tourType: null,
                minDuration: null,
                maxDuration: null,
                minPrice: null,
                maxPrice: null,
                page: 1,
                pageSize: 10,
                cancellationToken: cancellationToken);


            var images = await locationImageRepository.GetImagesByLocationIdAsync(
                 request.LocationId, cancellationToken);


            return new GetLocationDetailsResult(
                location.Id,
                location.Name,
                location.Description,
                location.Rating,
                location.Category.ToString(),
                location.MainImageUrl.Value,
                images.Select(i => i.ImageUrl.Value).ToList(),
                location.Coordinates?.Latitude,
                location.Coordinates?.Longitude,
                packages.Select(p => new LocationPackageSummaryResult(
                    p.Id,
                    p.Title,
                    p.AdultPrice,
                    p.DurationHours,
                    p.MainImageUrl.Value,
                    p.TourTypes.ToString(),
                    p.Views)).ToList());
        }
    }
}
