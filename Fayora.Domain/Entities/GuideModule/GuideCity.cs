using Fayora.Domain.Entities.Shared;

namespace Fayora.Domain.Entities.TourGuide;

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
