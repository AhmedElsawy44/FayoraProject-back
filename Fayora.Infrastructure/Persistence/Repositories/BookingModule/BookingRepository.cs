using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Enums.BookingModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.BookingModule;

public class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    public void AddBooking(Booking booking)
    {
        context.Bookings.Add(booking);
    }

    public Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken).AsTask();
    }

    public Task<bool> HasOverlapAsync(Guid serviceId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
    {
        return context.Bookings
            .AsNoTracking()
            .AnyAsync(
                b => b.ServiceId == serviceId
                     && b.BookingStatus != BookingStatus.Cancelled
                     && startDateTime < b.EndDate
                     && endDateTime > b.StartDate,
                cancellationToken);
    }
}
