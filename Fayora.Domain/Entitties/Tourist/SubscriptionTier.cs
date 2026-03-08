namespace Fayora.Domain.Entities.Subscriptions;

public class SubscriptionTier : BaseEntity<int>
{
    public string PlanCode { get; private set; } = null!;
    public string PlanName { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int AIChatLimit { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private SubscriptionTier() { }
}