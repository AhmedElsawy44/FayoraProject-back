namespace Fayora.Domain.Entities.TouristModule;

public class SubscriptionTier : BaseEntity<int>
{
    public string PlanCode { get; private set; } = null!;
    public string PlanName { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int AIChatLimit { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public SubscriptionTier(string planCode, string planName, decimal price, int aIChatLimit, bool isActive)
    {
        PlanCode = planCode;
        PlanName = planName;
        Price = price;
        AIChatLimit = aIChatLimit;
        IsActive = isActive;
    }

    public void ChangePrice(decimal newPrice) => Price = newPrice;
    public void ChangeName(string planeName) => PlanName = planeName;
    public void UpdateAIChatLimit(int newAiChatLimit) => AIChatLimit = newAiChatLimit;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private SubscriptionTier() { }
}