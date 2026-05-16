using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{
    public record GetActivePackagesQuery(
        string? Search,
        int? LocationId,
        string? TourType,
        int? MinDuration,
        int? MaxDuration,
        decimal? MinPrice,
        decimal? MaxPrice,
        int Page,
        int PageSize) : IQuery<Result<GetActivePackagesResult>>;
}
