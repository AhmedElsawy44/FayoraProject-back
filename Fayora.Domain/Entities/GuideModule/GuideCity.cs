using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Domain.Entities.GuideModule;

public class GuideCity : BaseEntity<Guid>
{
    public Guid GuideId { get; init; }
    public int CityId { get; init; }
    public City City { get; init; } = default!;

    public GuideCity(Guid guideId, int cityId)
    {
        GuideId = guideId;
        CityId = cityId;
    }

    private GuideCity() { }
}
