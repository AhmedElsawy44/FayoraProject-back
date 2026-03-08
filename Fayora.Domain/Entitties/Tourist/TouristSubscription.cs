using Fayora.Domain.Enums;

namespace Fayora.Domain.Entitties.Tourist;

public class TouristSubscription : BaseEntity<Guid>
{
    public Guid TouristProfileId { get; init; }
    public int PlanId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public SubscriptionStatus Status { get; init; }
    public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
    public bool AutoRenew { get; private set; }
    public bool IsExpired() => DateTime.UtcNow > EndDate;

    public static TouristSubscription Create(
        Guid touristProfileId,
        int planId,
        int durationInMonths,
        bool autoRenew)
    {
        return new TouristSubscription
        {
            Id = Guid.CreateVersion7(),
            TouristProfileId = touristProfileId,
            PlanId = planId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(durationInMonths),
            Status = SubscriptionStatus.Active,
            AutoRenew = autoRenew,
        };
    }

    private TouristSubscription() { }
}
