using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourGuideModule.GetPackageAccommodation
{
    public record GetPackageAccommodationResponse(
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
        string Amenities,
        string Meals
    );
}
