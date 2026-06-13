using Fayora.Domain.Enums.BookingModule;
using System;

namespace Fayora.Domain.Entities.Booking;

public class ProviderPayout : BaseEntity<Guid>
{
    public Guid ProviderId { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset PayoutDate { get; init; }
    public PayoutStatus Status { get; private set; } = PayoutStatus.Completed;

    public static ProviderPayout Create(Guid providerId, decimal amount)
    {
        return new ProviderPayout
        {
            Id = Guid.CreateVersion7(),
            ProviderId = providerId,
            Amount = amount,
            PayoutDate = DateTimeOffset.UtcNow,
            Status = PayoutStatus.Completed
        };
    }

    private ProviderPayout() { }
}
