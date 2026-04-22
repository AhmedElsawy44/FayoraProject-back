using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IHousingUnitRepository
{
    void AddUnit(HousingUnit housingUnit);
    Task<HousingUnit?> GetUnitByIdAsync(Guid unitId, UnitQueryOptions? options = null, CancellationToken cancellationToken = default);

    public record UnitQueryOptions(
        bool IncludeAmenities = false,
        bool IncludeImages = false,
        bool IsReadOnly = true
    );
}
