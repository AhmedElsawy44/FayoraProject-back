using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreatePackageOccurrences;

public record CreatePackageOccurrencesCommand(
    Guid PackageId,
    List<OccurrenceItemDto> Occurrences
) : ICommand<Result<Success>>;

public record OccurrenceItemDto(
    DateOnly Date,
    int AvailableSeats
);
