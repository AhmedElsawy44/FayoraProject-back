using System;

namespace Fayora.Contracts.ExploreModule;

public record ExploreItemResponse(
    string Id,
    string Title,
    string Type, // "location", "package", "accommodation", "guide"
    string ImageUrl,
    decimal? Rating,
    int ReviewCount,
    string? Subtitle,
    string? Price,
    string? Duration,
    string? Badge,
    bool IsWishlisted
);
