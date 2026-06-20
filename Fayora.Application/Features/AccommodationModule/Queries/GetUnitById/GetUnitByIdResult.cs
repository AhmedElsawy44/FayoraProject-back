using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;

public record GetUnitByIdResult(
    Guid UnitId,
    Guid OwnerId,
    string Title,
    string? Description,
    HousingType Type,
    int LocationId,
    string AddressDetails,
    GeoPoint Coordinates,
    int NumberOfRooms,
    int BedRooms,
    int BathRooms,
    int NumberOfBeds,
    int MaxGuests,
    TimeSpan CheckInTime,
    TimeSpan CheckOutTime,
    decimal PricePerNight,
    decimal DiscountedPricePerNight,
    decimal Rating,
    int ReviewCount,
    int Views,
    string? MainImageUrl,
    List<string> ImageUrls,
    List<Amenity> Amenities,
    DateTimeOffset CreatedAt,
    DateTime? AvailableStartDate,
    DateTime? AvailableEndDate,
    // Owner / Host info
    string? OwnerName,
    string? OwnerProfileImageUrl,
    bool IsSuperHost,
    int HostingSinceYear
);
