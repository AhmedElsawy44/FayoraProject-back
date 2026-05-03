using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.Booking;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class CalendarBlockRepository(ApplicationDbContext context) : ICalendarBlockRepository
{
    public void AddCalendarBlock(CalendarBlock block)
    {
        context.CalendarBlocks.Add(block);
    }
}
