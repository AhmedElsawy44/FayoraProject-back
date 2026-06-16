using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.SharedModule.Queries.GetActiveOffersByTarget
{
    public record GetActiveOffersByTargetQuery(
        Guid TargetId,
        OfferTargetType TargetType) : IQuery<Result<List<DiscountOfferResponse>>>;

}
