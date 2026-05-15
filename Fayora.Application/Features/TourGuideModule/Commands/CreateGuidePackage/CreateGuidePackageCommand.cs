using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.TourGuideModule.CreateGuidePackage;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public record CreateGuidePackageCommand
(
    string Title,
    string Description,
    TourType TourType,
    int DurationHours,
    decimal Longitude,
    decimal Latitude,
    TransportType TransportType,
    string? ArrivalNote,
    decimal AdultPrice,
    decimal ChildPrice,
    int MaxCapacity,
    List<int> IncludedIds,
    List<int> ExcludedIds,
    string MainImageUrl,
    string? VideoURL,
    List<string> ImageURLs,
    string? GuestRequirements,
    CancellationPolicy CancellationPolicy,
    List<ActivityDto> Activities,
    HashSet<int> LocationIds
) : ICommand<Result<CreateGuidePackageResult>>;
