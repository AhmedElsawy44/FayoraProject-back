namespace Fayora.Contracts.ExploreModule;

public record ExploreItemDto(
    string Id,
    string Type,
    string Title,
    string ImageUrl,
    bool IsFavorite,
    double CardHeight,
    double? Rating = null,
    string? PriceLabel = null,
    string? DurationLabel = null,
    string? LocationLabel = null,
    string? BadgeLabel = null
);
