using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{
    public record GetActivePackagesQuery(
        string? Search,
        int? LocationId,
        ProviderType? ProviderType,
        TourType? TourType,
        int? MinDuration,
        int? MaxDuration,
        decimal? MinPrice,
        decimal? MaxPrice,
        int Page,
        int PageSize) : IQuery<Result<GetActivePackagesResult>>;
}
