using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetMapPlaces
{
    public class GetMapPlacesQueryHandler(
        ILocationRepository locationRepository,
        IHousingUnitRepository housingUnitRepository)
        : IQueryHandler<GetMapPlacesQuery, Result<List<MapPlaceResult>>>
    {
        public async Task<Result<List<MapPlaceResult>>> Handle(
            GetMapPlacesQuery request,
            CancellationToken cancellationToken)
        {
            var mapPlaces = new List<MapPlaceResult>();

            // 1. Fetch tourist locations
            var (locations, _) = await locationRepository.GetAllLocationsAsync(
                category: null,
                minRating: null,
                search: null,
                page: 1,
                pageSize: 1000,
                ct: cancellationToken);

            if (locations != null)
            {
                foreach (var loc in locations)
                {
                    mapPlaces.Add(new MapPlaceResult(
                        Id: loc.Id.ToString(),
                        Name: loc.Name,
                        Latitude: loc.Coordinates?.Latitude ?? 0,
                        Longitude: loc.Coordinates?.Longitude ?? 0,
                        ImageUrl: loc.MainImageUrl?.Value ?? "",
                        Description: loc.Description ?? "",
                        Type: "public_place"
                    ));
                }
            }

            // 2. Fetch all active accommodations
            var housingUnits = await housingUnitRepository.GetUnitsAsync(
                type: null,
                searchTerm: null,
                cancellationToken: cancellationToken);

            if (housingUnits != null)
            {
                foreach (var unit in housingUnits)
                {
                    string typeString = MapHousingType(unit.Type);

                    mapPlaces.Add(new MapPlaceResult(
                        Id: unit.Id.ToString(),
                        Name: unit.Title,
                        Latitude: unit.Coordinates?.Latitude ?? 0,
                        Longitude: unit.Coordinates?.Longitude ?? 0,
                        ImageUrl: unit.MainImageUrl?.Value ?? "",
                        Description: unit.Description ?? "",
                        Type: typeString
                    ));
                }
            }

            return mapPlaces;
        }

        private static string MapHousingType(HousingType type)
        {
            return type switch
            {
                HousingType.Villa => "villa",
                HousingType.Hotel => "hotel",
                HousingType.Apartment => "apartment",
                HousingType.Camp => "camp",
                HousingType.EcoLodge => "ecolodge",
                _ => "hotel"
            };
        }
    }
}
