using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.SharedModule.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQueryHandler(
        IDiscountOfferRepository discountOfferRepository,
        IMapper mapper)
        : IQueryHandler<ValidatePromoCodeQuery, Result<DiscountOfferResponse>>
    {
        public async Task<Result<DiscountOfferResponse>> Handle(
            ValidatePromoCodeQuery request,
            CancellationToken cancellationToken)
        {
            var offer = await discountOfferRepository.GetActiveByCodeAndTargetAsync(
                request.Code,
                request.TargetId,
                request.TargetType,
                cancellationToken);

            if (offer is null)
            {
                return Error.NotFound("DiscountOffer.PromoNotFound", "The promo code is invalid, expired, or does not apply to this target.");
            }

            return mapper.Map<DiscountOfferResponse>(offer);
        }
    }
}
