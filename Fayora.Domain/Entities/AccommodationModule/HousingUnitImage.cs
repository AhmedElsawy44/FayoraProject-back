namespace Fayora.Domain.Entities.AccommodationModule;

public class HousingUnitImage
{
    public Guid Id { get; private set; }
    public Guid UnitId { get; private set; }
    public string ImageUrl { get; private set; }

    public HousingUnitImage(Guid housingUnitId, string imageUrl)
    {
        Id = Guid.NewGuid();
        UnitId = housingUnitId;
        ImageUrl = imageUrl;
    }

    private HousingUnitImage() { }
}