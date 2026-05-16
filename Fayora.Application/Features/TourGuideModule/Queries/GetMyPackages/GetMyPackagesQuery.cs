using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages
{
    public record GetMyPackagesQuery(
    ItemStatus? Status,
    int Page,
    int PageSize) : IQuery<Result<GetMyPackagesResult>>;
}
