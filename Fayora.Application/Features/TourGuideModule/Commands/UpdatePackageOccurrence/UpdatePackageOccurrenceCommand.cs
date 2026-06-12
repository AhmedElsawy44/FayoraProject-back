using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdatePackageOccurrence;

public record UpdatePackageOccurrenceCommand(
    Guid PackageId,
    Guid OccurrenceId,
    DateOnly NewDate,
    int NewAvailableSeats
) : ICommand<Result<Success>>;
