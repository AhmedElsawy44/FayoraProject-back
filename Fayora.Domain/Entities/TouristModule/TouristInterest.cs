namespace Fayora.Domain.Entities.TouristModule;

public class TouristInterest : BaseEntity<int>
{
    public Guid TouristId { get; init; }
    public int InterestId { get; init; }

    public TouristInterest(Guid touristId, int interestId)
    {
        TouristId = touristId;
        InterestId = interestId;
    }

    private TouristInterest() { }
}
