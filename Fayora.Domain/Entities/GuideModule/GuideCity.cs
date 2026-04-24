using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Domain.Entities.GuideModule;

public class GuideCity
{
    public Guid GuideId { get; init; }
    public int CityId { get; init; }

    public TourGuide TourGuide { get; private set; } = null!;
    public City City { get; private set; } = null!;

    public GuideCity(Guid guideId, int cityId)
    {
        GuideId = guideId;
        CityId = cityId;
    }

    private GuideCity() { }
}
