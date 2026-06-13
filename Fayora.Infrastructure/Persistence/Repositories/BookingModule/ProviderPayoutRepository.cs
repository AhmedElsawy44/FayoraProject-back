using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Domain.Entities.Booking;

namespace Fayora.Infrastructure.Persistence.Repositories.BookingModule;

public class ProviderPayoutRepository(ApplicationDbContext context) : IProviderPayoutRepository
{
    public void Add(ProviderPayout payout)
    {
        context.ProviderPayouts.Add(payout);
    }
}
