using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IMasterAmenityRepository
{
    Task<List<MasterAmenity>> GetAmenitiesByIdsAsync(List<int> amenityIds, CancellationToken cancellationToken);
    Task<List<MasterAmenity>> GetAllAmenitiesAsync(CancellationToken cancellationToken);
}
