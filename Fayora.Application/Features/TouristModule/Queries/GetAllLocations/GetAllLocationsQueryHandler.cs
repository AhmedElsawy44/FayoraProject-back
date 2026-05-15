using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllLocations
{
    public class GetAllLocationsQueryHandler(
        ILocationRepository locationRepository)
        : IQueryHandler<GetAllLocationsQuery, Result<GetAllLocationsResult>>
    {
        public async Task<Result<GetAllLocationsResult>> Handle(
              GetAllLocationsQuery request,
              CancellationToken cancellationToken)
        {
            var (items, totalCount) = await locationRepository.GetAllLocationsAsync(
                request.Category,
                request.MinRating,
                request.Search,
                request.Page,
                request.PageSize,
                cancellationToken);

            return new GetAllLocationsResult(
                items.Select(l => new LocationSummaryResult(
                    l.Id,
                    l.Name,
                    l.MainImageUrl.Value,
                    l.Rating,
                    l.Category.ToString())).ToList(),
                totalCount,
                request.Page,
                request.PageSize);
        }
    }
}
