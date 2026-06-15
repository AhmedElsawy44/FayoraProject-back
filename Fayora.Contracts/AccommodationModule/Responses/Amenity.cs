namespace Fayora.Contracts.AccommodationModule.Responses;

public record Amenity(
    int Id,
    string Name,
    string? IconUrl,
    string Category
);
