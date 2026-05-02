using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Domain.Entities.Booking;

namespace Fayora.Infrastructure.Persistence.Repositories.BookingModule;

public class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    public void AddBooking(Booking booking)
    {
        context.Bookings.Add(booking);
    }
}
