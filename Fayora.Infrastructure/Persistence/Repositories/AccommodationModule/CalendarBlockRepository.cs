using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class CalendarBlockRepository(ApplicationDbContext context) : ICalendarBlockRepository
{
    public void AddCalendarBlock(CalendarBlock block)
    {
        context.CalendarBlocks.Add(block);
    }

    public async Task RemoveByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        await context.CalendarBlocks
            .Where(x => x.BookingId == bookingId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public void RemoveCalendarBlock(CalendarBlock calendarBlock)
    {
        context.CalendarBlocks.Remove(calendarBlock);
    }
}
