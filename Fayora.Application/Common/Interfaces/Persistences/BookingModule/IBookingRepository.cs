using Fayora.Domain.Entities.Booking;

namespace Fayora.Application.Common.Interfaces.Persistences.BookingModule;

public interface IBookingRepository
{
    void AddBooking(Booking booking);
    Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid serviceId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);
}
