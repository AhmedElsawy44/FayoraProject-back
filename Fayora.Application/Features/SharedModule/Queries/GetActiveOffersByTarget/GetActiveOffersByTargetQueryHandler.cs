using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.SharedModule.Queries.GetActiveOffersByTarget
{
    public class GetActiveOffersByTargetQueryHandler(
        IDiscountOfferRepository discountOfferRepository,
        IMapper mapper)
        : IQueryHandler<GetActiveOffersByTargetQuery, Result<List<DiscountOfferResponse>>>
    {
        public async Task<Result<List<DiscountOfferResponse>>> Handle(
            GetActiveOffersByTargetQuery request,
            CancellationToken cancellationToken)
        {
            var offers = await discountOfferRepository.GetActiveByTargetAsync(
                request.TargetId,
                request.TargetType,
                cancellationToken);

            return mapper.Map<List<DiscountOfferResponse>>(offers);
        }
    }
}
