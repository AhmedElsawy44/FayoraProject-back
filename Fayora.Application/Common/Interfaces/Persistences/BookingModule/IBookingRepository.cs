using Fayora.Domain.Entities.Booking;

namespace Fayora.Application.Common.Interfaces.Persistences.BookingModule;

public interface IBookingRepository
{
    void AddBooking(Booking booking);
}
