using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.TourGuideModule.CreateGuidePackage;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateGuidePackage;

public record UpdateGuidePackageCommand(
    Guid PackageId,
    string Title,
    string Description,
    TourType TourType,
    int DurationHours,
    int NumOfDays,
    List<NightDto>? Nights,
    List<MeetingPointDto> MeetingPoints,
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
    HashSet<int> LocationIds,
    List<OptionalActivityDto>? OptionalActivities,
    bool HasGroupDiscount = false,
    int? GroupDiscountMinPeople = null,
    decimal? GroupDiscountPercent = null
) : ICommand<Result<Guid>>;
