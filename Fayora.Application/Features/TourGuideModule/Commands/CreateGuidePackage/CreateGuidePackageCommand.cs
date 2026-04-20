using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public record CreateGuidePackageCommand
(
    string Title,
    string Description,
    string TourType,
    int DurationHours,
    decimal Longitude,
    decimal Latitude,
    string TransportType,
    string ArrivalNote,
    decimal AdultPrice,
    decimal ChildPrice,
    int MaxCapacity,
    List<string> IncludedItems,
    List<string> ExcludedItems,
    string MainImageUrl,
    List<string> ImageURLs,
    string VideoURL,
    string CancellationPolicy,
    string GuestRequirements
) : IRequest<Result<CreateGuidePackageResult>>;
