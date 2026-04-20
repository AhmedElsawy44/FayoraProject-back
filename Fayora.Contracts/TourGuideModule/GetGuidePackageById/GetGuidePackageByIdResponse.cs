using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Fayora.Contracts.TourGuideModule.GetGuidePackageById;

public record GetGuidePackageByIdResponse
(
    Guid Id,
    Guid GuideId,
    string Title,
    string Description,
    string TourType,
    int DurationHours,
    string Longitude,
    string Latitude,
    string? ArrivalNote,
    string TransportType,
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
