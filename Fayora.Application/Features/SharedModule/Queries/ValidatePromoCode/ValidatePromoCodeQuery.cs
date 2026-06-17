using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using System;

namespace Fayora.Application.Features.SharedModule.Queries.ValidatePromoCode
{
    public record ValidatePromoCodeQuery(
        string Code,
        Guid TargetId,
        OfferTargetType TargetType) : IQuery<Result<DiscountOfferResponse>>;
}
