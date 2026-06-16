using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageAccommodation
{
    public class GetPackageAccommodationQueryHandler(
        IPackageAccommodationRepository packageAccommodationRepository)
        : IQueryHandler<GetPackageAccommodationQuery, Result<GetPackageAccommodationResult>>
    {
        public async Task<Result<GetPackageAccommodationResult>> Handle(
            GetPackageAccommodationQuery request,
            CancellationToken cancellationToken)
        {
            var accommodation = await packageAccommodationRepository.GetByIdAsync(
                request.PackageAccommodationId, cancellationToken);
            if (accommodation is null)
                return Error.NotFound("PackageAccommodation.NotFound", "Accommodation not found.");

            return new GetPackageAccommodationResult(
                accommodation.Id,
                accommodation.Name,
                accommodation.Description,
                accommodation.Type.ToString(),
                accommodation.MainImageUrl.Value,
                accommodation.GalleryImages.Select(img => img.Value).ToList(),
                accommodation.Location.Latitude,
                accommodation.Location.Longitude,
                accommodation.CheckInTime,
                accommodation.CheckOutTime,
                accommodation.Amenities.ToString(),
                accommodation.Meals.ToString());
        }
    }
}
