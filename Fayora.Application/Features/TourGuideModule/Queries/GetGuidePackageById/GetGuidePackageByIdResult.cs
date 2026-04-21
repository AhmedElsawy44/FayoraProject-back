using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetGuidePackageById;

public record GetGuidePackageByIdResult(
    Guid Id,
    Guid GuideId,
    string Title,
    string Description,
    TourType TourType,
    int DurationHours,
    GeoPoint MeetingPoint,
    string? ArrivalNote,
    TransportType TransportType,
    int MaxCapacity,
    int BookingsCount,
    int AvailableSpots,
    decimal AdultPrice,
    decimal ChildPrice,
    bool IsActive,
    int Views,
    string? MainImageUrl,
    string? MainVideoUrl,
    string? GuestRequirements,
    IEnumerable<string> Images,
    IEnumerable<string> IncludedItems,
    IEnumerable<string> ExcludedItems
);
