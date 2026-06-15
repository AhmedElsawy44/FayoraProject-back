using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;

public interface IMasterAmenityRepository
{
    Task<MasterAmenity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<MasterAmenity>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default);
    Task<List<MasterAmenity>> GetAllAsync(bool IsActive = true, CancellationToken cancellationToken = default);
    void Add(MasterAmenity amenity);
    void Delete(MasterAmenity amenity);
}