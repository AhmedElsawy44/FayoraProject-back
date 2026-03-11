namespace Fayora.Domain.Entitties.Accommodation;

public class UnitAmenity : BaseEntity<Guid>
{
    public Guid UnitId { get; init; }
    public int AmenityId { get; init; }

    public UnitAmenity(Guid unitId, int amenityId)
    {
        Id = Guid.CreateVersion7();
        UnitId = unitId;
        AmenityId = amenityId;
    }

    public UnitAmenity() { }
}
