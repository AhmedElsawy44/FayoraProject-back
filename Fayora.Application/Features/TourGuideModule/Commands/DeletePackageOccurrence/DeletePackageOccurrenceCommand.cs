using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeletePackageOccurrence;

public record DeletePackageOccurrenceCommand(
    Guid PackageId,
    Guid OccurrenceId
) : ICommand<Result<Success>>;
