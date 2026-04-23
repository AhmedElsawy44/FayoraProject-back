using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.AccommodationModule;

public class HousingUnitImage
{
    public Guid Id { get; init; }
    public Guid UnitId { get; init; }
    public FileUrl ImageUrl { get; init; } = null!;

    public HousingUnitImage(Guid housingUnitId, FileUrl imageUrl)
    {
        Id = Guid.NewGuid();
        UnitId = housingUnitId;
        ImageUrl = imageUrl;
    }

    private HousingUnitImage() { }
}