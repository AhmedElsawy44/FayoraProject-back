namespace Fayora.Contracts.AccommodationModule.Responses;

public record Amenity(
    string Name,
    string IconUrl,
    string Category
);

public record GetUnitByIdResponse(
    Guid UnitId,
    Guid OwnerId,
    string Title,
    string? Description,
    string Type,
    int LocationId,
    string AddressDetails,
    string Coordinates,
    int NumberOfRooms,
    int BedRooms,
    int BathRooms,
    int NumberOfBeds,
    int MaxGuests,
    string CheckInTime,
    string CheckOutTime,
    decimal PricePerNight,
    decimal DiscountedPricePerNight,
    decimal Rating,
    int ReviewCount,
    int Views,
    string? MainImageUrl,
    List<string> ImageUrls,
    List<Amenity> Amenities,
    DateTimeOffset CreatedAt
);