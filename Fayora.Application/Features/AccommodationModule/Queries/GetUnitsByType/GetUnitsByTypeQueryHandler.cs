using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitsByType
{

    public class GetUnitsByTypeQueryHandler(
        IHousingUnitRepository housingUnitRepository,
        IMapper mapper)
        : IQueryHandler<GetUnitsByTypeQuery, Result<List<GetAllUnitsByTypeResponse>>>
    {
        public async Task<Result<List<GetAllUnitsByTypeResponse>>> Handle(
            GetUnitsByTypeQuery request,
            CancellationToken cancellationToken)
        {
            var units = await housingUnitRepository.GetUnitsByTypeAsync(
                request.Type,
                cancellationToken);

            var response = mapper.Map<List<GetAllUnitsByTypeResponse>>(units);

            return response;
        }
    }
}
