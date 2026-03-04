using Fayora.Domain.Entities.Identity;

namespace Fayora.Domain.Policies;

public static class ViolationPolicy
{
    public static void Apply(User user)
    {
        if (user.LastViolationDate.HasValue &&
            DateTimeOffset.UtcNow >
            user.LastViolationDate.Value.AddDays(30))
        {
            user.ResetViolations();
        }

        user.IncreaseViolationInternal();
    }
}
