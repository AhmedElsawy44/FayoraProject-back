namespace Fayora.Domain.Entitties.Tourist;

public class TouristInterest : BaseEntity<int>
{
    public Guid TouristId { get; init; }
    public int InterestId { get; init; }

    public TouristInterest(Guid touristId, int interestId)
    {
        TouristId = touristId;
        InterestId = interestId;
    }

    public TouristInterest() { }
}
