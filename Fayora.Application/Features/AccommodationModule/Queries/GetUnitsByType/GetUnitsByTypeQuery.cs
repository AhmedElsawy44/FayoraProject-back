using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitsByType
{
    public record GetUnitsByTypeQuery(HousingType Type) : IQuery<Result<List<GetAllUnitsByTypeResponse>>>;
}
