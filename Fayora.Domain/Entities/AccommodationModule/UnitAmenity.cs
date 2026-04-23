namespace Fayora.Domain.Entities.AccommodationModule;

public class UnitAmenity 
{
    public Guid UnitId { get; init; }
    public int AmenityId { get; init; }

    public UnitAmenity(Guid unitId, int amenityId)
    {
        UnitId = unitId;
        AmenityId = amenityId;
    }

    private UnitAmenity() { }
}
