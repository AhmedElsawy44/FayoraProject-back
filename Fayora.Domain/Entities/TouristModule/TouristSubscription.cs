using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TouristModule;
using Fayora.Domain.Errors;

namespace Fayora.Domain.Entities.TouristModule;

public class TouristSubscription : BaseEntity<Guid>
{
    public Guid UserId { get; init; }
    public int PlanId { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.UtcNow;
    public int AIChatLimit { get; init; }
    public int AiMessagesUsed { get; private set; } = 0;
    public bool AutoRenew { get; private set; }
    public bool IsExpired() => DateTime.UtcNow > EndDate;

    public TouristSubscription(
        Guid userId,
        int planId,
        int durationInMonths,
        int aIChatLimit,
        bool autoRenew)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        PlanId = planId;
        StartDate = DateTimeOffset.UtcNow;
        EndDate = StartDate.AddMonths(durationInMonths);
        Status = SubscriptionStatus.Active;
        AutoRenew = autoRenew;
        AIChatLimit = aIChatLimit;
    }

    public void Renew(int durationInMonths)
    {
        EndDate = EndDate.AddMonths(durationInMonths);
        AiMessagesUsed = 0;
        Status = SubscriptionStatus.Active;
    }

    public bool CanSendAiMessage() => Status == SubscriptionStatus.Active && !IsExpired() && AiMessagesUsed < AIChatLimit;

    public Result<Success> IncrementAiUsage()
    {
        if (!CanSendAiMessage())
            return TouristErrors.AiQuotaExceeded;

        AiMessagesUsed++;
        return Result.Success;
    }

    public void CancelSubscription()
    {
        AutoRenew = false;
        Status = SubscriptionStatus.Cancelled;
    }

    private TouristSubscription() { }
}
