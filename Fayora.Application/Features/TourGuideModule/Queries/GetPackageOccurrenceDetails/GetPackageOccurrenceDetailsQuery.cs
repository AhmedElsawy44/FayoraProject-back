using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageOccurrenceDetails;

public record GetPackageOccurrenceDetailsQuery(
    Guid PackageId,
    Guid OccurrenceId
) : IQuery<Result<GetPackageOccurrenceDetailsResult>>;
