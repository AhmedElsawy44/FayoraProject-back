using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using System.Collections.Generic;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetMyHousingUnits
{
    public record GetMyHousingUnitsQuery() : IQuery<Result<List<GetOwnerHousingUnitsResponse>>>;
}
