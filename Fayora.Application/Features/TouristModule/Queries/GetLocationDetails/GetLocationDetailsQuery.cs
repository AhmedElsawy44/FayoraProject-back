using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Queries.GetLocationDetails
{
    public record GetLocationDetailsQuery(int LocationId) : IQuery<Result<GetLocationDetailsResult>>;
}
