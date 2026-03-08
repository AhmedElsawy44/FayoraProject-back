using Fayora.Domain.Enums;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entitties.Tourist;

public class TouristProfile : BaseEntity<Guid>
{
    public Guid UserId { get; init; }
    public int SubscriptionTierId { get; private set; }

    public TravelStyle TravelStyle { get; private set; }
    public BudgetTier BudgetTier { get; private set; }
    public TravelCompanionType TravelCompanionType { get; private set; }

    public float[]? EmbeddingVector { get; private set; }

    public GeoPoint LastLocation { get; private set; } = new GeoPoint(0, 0);
    public DateTimeOffset? LastLocationUpdate { get; private set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool OnboardingComplete { get; private set; }
}
