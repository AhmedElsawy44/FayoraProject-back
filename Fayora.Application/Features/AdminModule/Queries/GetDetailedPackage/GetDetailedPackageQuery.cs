using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedPackage;

public record GetDetailedPackageQuery(Guid PackageId) : IQuery<Result<GetDetailedPackageResult>>;

public record GetDetailedPackageResult(
    Guid PackageId,
    Guid UserId,
    string UserName,
    string Title,
    string Description,
    string TourType,
    int DurationInHours,
    int MaxCapacity,
    decimal AdultPrice,
    decimal ChildPrice,
    string MainImageUrl,
    string? MainVideoUrl,
    string? GuestRequirements,
    string CancellationPolicy,
    List<int> IncluededItems,
    List<int> ExcludedItems,
    GeoPoint MeetingPoint,
    string? ArrivalNote,
    string TransportType,
    List<string> ImageUrls
);