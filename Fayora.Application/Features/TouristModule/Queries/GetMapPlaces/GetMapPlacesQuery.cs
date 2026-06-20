using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetMapPlaces
{
    public record GetMapPlacesQuery() : IQuery<Result<List<MapPlaceResult>>>;
}
