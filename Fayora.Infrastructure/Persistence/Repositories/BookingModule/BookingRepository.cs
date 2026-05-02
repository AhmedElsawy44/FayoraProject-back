using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Domain.Entities.Booking;

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
}
