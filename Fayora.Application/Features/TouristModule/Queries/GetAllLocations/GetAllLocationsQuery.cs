using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllLocations
{
    public record GetAllLocationsQuery(
        LocationCategory? Category,
        decimal? MinRating,
        string? Search,
        int Page = 1,
        int PageSize = 10) : IQuery<Result<GetAllLocationsResult>>;
}
