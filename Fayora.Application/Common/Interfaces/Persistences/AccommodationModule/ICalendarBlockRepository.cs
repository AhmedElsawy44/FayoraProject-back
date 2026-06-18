using Fayora.Domain.Entities.Booking;

namespace Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;

public interface ICalendarBlockRepository
{
    void AddCalendarBlock(CalendarBlock block);

    Task RemoveByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);

    void RemoveCalendarBlock(CalendarBlock calendarBlock);

    Task<bool> HasOverlapAsync(Guid serviceId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);
}
