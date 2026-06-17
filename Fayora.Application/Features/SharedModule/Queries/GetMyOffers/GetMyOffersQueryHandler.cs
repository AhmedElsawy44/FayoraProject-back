using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Entities.SharedModule;
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
            var context = clientContextProvider.GetContext();
            var ownerId = context.UserId;
            var isAdmin = context.Roles.Contains("Admin");

            List<DiscountOffer> offers;
            if (isAdmin)
            {
                offers = await discountOfferRepository.GetAllOffersAsync(cancellationToken);
            }
            else
            {
                offers = await discountOfferRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
            }

            var mapped = mapper.Map<List<DiscountOfferResponse>>(offers);

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<DiscountOfferStatus>(request.Status, true, out var parsedStatus))
            {
                mapped = mapped.Where(o => o.Status == parsedStatus.ToString()).ToList();
            }

            return mapped;
        }
    }
}
