using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageAccommodation
{
    public record GetPackageAccommodationResult(
        Guid Id,
        string Name,
        string? Description,
        string Type,
        string MainImageUrl,
        List<string> GalleryImages,
        decimal Latitude,
        decimal Longitude,
        TimeOnly CheckInTime,
        TimeOnly CheckOutTime,
        string Amenities
    );
}
