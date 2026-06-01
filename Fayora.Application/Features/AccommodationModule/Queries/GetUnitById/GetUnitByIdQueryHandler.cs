//using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
//using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
//using Fayora.Application.Features.AccommodationModule.Common;
//using Fayora.Domain.Common.Results;
//using Fayora.Domain.Enums.SharedModule;
//using MediatR;

//namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;

//public class GetUnitByIdQueryHandler(
//    IHousingUnitRepository housingUnitRepository,
//    IMasterAmenityRepository masterAmenityRepository,
//    IDiscountOfferRepository discountOfferRepository) : IRequestHandler<GetUnitByIdQuery, Result<GetUnitByIdResult>>
//{
//    public async Task<Result<GetUnitByIdResult>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
//    {
//        var option = new IHousingUnitRepository.UnitQueryOptions(
//            IncludeAmenities: true,
//            IncludeImages: true,
//            IsReadOnly: true
//        );

//        var unit = await housingUnitRepository.GetUnitByIdAsync(request.UnitId, option, cancellationToken);

//        if (unit is null) return AccommodationErrors.UnitNotFound;

//        var amenityIds = unit.Amenities.Select(a => a.AmenityId).ToList();

//        var amenities = amenityIds.Count != 0
//            ? await masterAmenityRepository.GetAmenitiesByIdsAsync(amenityIds, cancellationToken)
//            : [];

//        decimal discountedPricePerNight = unit.PricePerNight;
//        var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
//            unit.Id, OfferTargetType.HousingUnit, cancellationToken);
//        var offer = activeOffers.FirstOrDefault();
//        if (offer is not null)
//        {
//            var discountResult = offer.ApplyTo(unit.PricePerNight);
//            if (!discountResult.IsError)
//            {
//                discountedPricePerNight = discountResult.Value;
//            }
//        }

//        return new GetUnitByIdResult(
//            unit.Id,
//            unit.OwnerId,
//            unit.Title,
//            unit.Description,
//            unit.Type,
//            unit.LocationId,
//            unit.AddressDetails,
//            unit.Coordinates,
//            unit.NumberOfRooms,
//            unit.BedRooms,
//            unit.BathRooms,
//            unit.NumberOfBeds,
//            unit.MaxGuests,
//            unit.CheckInTime,
//            unit.CheckOutTime,
//            unit.PricePerNight,
//            discountedPricePerNight,
//            unit.Rating,
//            unit.ReviewCount,
//            unit.Views,
//            unit.MainImageUrl,
//            unit.Images.Select(i => i.ImageUrl).ToList(),
//            amenities,
//            unit.CreatedAt
//        );
//    }
//}