using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;

public class GetUnitByIdQueryHandler(
    IHousingUnitRepository housingUnitRepository,
    IHousingUnitImageRepository housingUnitImageRepository,
    IDiscountOfferRepository discountOfferRepository)
    : IRequestHandler<GetUnitByIdQuery, Result<GetUnitByIdResult>>
{
    public async Task<Result<GetUnitByIdResult>> Handle(
        GetUnitByIdQuery request,
        CancellationToken cancellationToken)
    {
        var options = new IHousingUnitRepository.UnitQueryOptions(IsReadOnly: true, IncludeAmenties: true);

        var unit = await housingUnitRepository.GetUnitByIdAsync(
            request.UnitId, options, cancellationToken);

        if (unit is null)
            return AccommodationErrors.UnitNotFound;


        var images = await housingUnitImageRepository.GetByUnitIdAsync(
            request.UnitId, cancellationToken);

        var imageUrls = images.Select(i => i.ImageUrl.Value).ToList();

        var amenitiesResult = unit.Amenities
            .Select(a => new Amenity(
                a.Id,
                a.Name,
                a.Icon.Value,
                a.Category.ToString()
            ))
            .ToList();


        decimal discountedPricePerNight = unit.PricePerNight;
        var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
            unit.Id, OfferTargetType.HousingUnit, cancellationToken);
        var offer = activeOffers.FirstOrDefault();
        if (offer is not null)
        {
            var discountResult = offer.ApplyTo(unit.PricePerNight);
            if (!discountResult.IsError)
                discountedPricePerNight = discountResult.Value;
        }

        return new GetUnitByIdResult(
            unit.Id,
            unit.OwnerId,
            unit.Title,
            unit.Description,
            unit.Type,
            unit.LocationId,
            unit.AddressDetails,
            unit.Coordinates,
            unit.NumberOfRooms,
            unit.BedRooms,
            unit.BathRooms,
            unit.NumberOfBeds,
            unit.MaxGuests,
            unit.CheckInTime,
            unit.CheckOutTime,
            unit.PricePerNight,
            discountedPricePerNight,
            unit.Rating,
            unit.ReviewCount,
            unit.Views,
            unit.MainImageUrl.Value,
            imageUrls,
            amenitiesResult,
            unit.CreatedAt,
            unit.AvailableStartDate,
            unit.AvailableEndDate
        );
    }
}