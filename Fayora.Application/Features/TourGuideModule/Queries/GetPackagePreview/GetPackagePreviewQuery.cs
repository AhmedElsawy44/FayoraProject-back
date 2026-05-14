using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackagePreview
{
    public record GetPackagePreviewQuery(Guid PackageId) : IQuery<Result<PackagePreviewResult>>;
}
