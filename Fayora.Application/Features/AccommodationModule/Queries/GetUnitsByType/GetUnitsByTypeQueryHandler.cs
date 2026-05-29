using AutoMapper;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitsByType
{

    public class GetUnitsByTypeQueryHandler(
        IHousingUnitRepository housingUnitRepository,
        IDiscountOfferRepository discountOfferRepository,
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

            for (int i = 0; i < response.Count; i++)
            {
                var unit = units[i];
                var item = response[i];

                decimal discountedPrice = unit.PricePerNight;
                var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                    unit.Id, OfferTargetType.HousingUnit, cancellationToken);
                var offer = activeOffers.FirstOrDefault();
                if (offer is not null)
                {
                    var discountResult = offer.ApplyTo(unit.PricePerNight);
                    if (!discountResult.IsError)
                    {
                        discountedPrice = discountResult.Value;
                    }
                }

                response[i] = item with { DiscountedPricePerNight = discountedPrice };
            }

            return response;
        }
    }
}
