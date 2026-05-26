using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.SharedModule.Queries.GetMyOffers
{
    public class GetMyOffersQueryHandler(
        IClientContextProvider clientContextProvider,
        IDiscountOfferRepository discountOfferRepository,
        IMapper mapper)
        : IQueryHandler<GetMyOffersQuery, Result<List<DiscountOfferResponse>>>
    {
        public async Task<Result<List<DiscountOfferResponse>>> Handle(
            GetMyOffersQuery request,
            CancellationToken cancellationToken)
        {
            var ownerId = clientContextProvider.GetContext().UserId;

            var offers = await discountOfferRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

            return mapper.Map<List<DiscountOfferResponse>>(offers);
        }
    }
}
