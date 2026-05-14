using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails
{
    public record GetPackageDetailsQuery(Guid PackageId) : IQuery<Result<PackageDetailsResult>>;
}
