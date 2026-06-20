using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetMyHousingUnits
{
    public class GetMyHousingUnitsQueryHandler(
        IClientContextProvider clientContextProvider,
        IHousingUnitRepository housingUnitRepository,
        IHousingUnitImageRepository housingUnitImageRepository)
        : IQueryHandler<GetMyHousingUnitsQuery, Result<List<GetOwnerHousingUnitsResponse>>>
    {
        public async Task<Result<List<GetOwnerHousingUnitsResponse>>> Handle(
            GetMyHousingUnitsQuery request,
            CancellationToken cancellationToken)
        {
            var ownerId = clientContextProvider.GetContext().UserId;

            var units = await housingUnitRepository.GetUnitsByOwnerIdAsync(ownerId, cancellationToken);

            if (!units.Any())
            {
                return new List<GetOwnerHousingUnitsResponse>();
            }

            var unitIds = units.Select(u => u.Id).ToList();
            var images = await housingUnitImageRepository.GetByUnitIdsAsync(unitIds, cancellationToken);
            var imagesByUnit = images.GroupBy(i => i.UnitId).ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl.Value).ToList());

            var response = units.Select(u => new GetOwnerHousingUnitsResponse(
                u.Id,
                u.Title,
                u.Description ?? string.Empty,
                u.LocationId,
                u.AddressDetails,
                (double)u.Coordinates.Latitude,
                (double)u.Coordinates.Longitude,
                u.Type.ToString(),
                u.PricePerNight,
                u.NumberOfRooms,
                u.BedRooms,
                u.BathRooms,
                u.NumberOfBeds,
                u.MaxGuests,
                u.CheckInTime.ToString(@"hh\:mm"),
                u.CheckOutTime.ToString(@"hh\:mm"),
                u.MainImageUrl.Value,
                u.VerificationDocumentUrl?.Value,
                imagesByUnit.TryGetValue(u.Id, out var imgList) ? imgList : new List<string>(),
                u.Amenities.Select(a => a.Id.ToString()).ToList(),
                u.Status.ToString(),
                u.AvailableStartDate,
                u.AvailableEndDate
            )).ToList();

            return response;
        }
    }
}
