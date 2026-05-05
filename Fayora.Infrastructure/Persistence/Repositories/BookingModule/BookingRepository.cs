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

    // For guide booking
    public async Task<bool> HasGuideBookingOnDateAsync(
    Guid guideId,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        var startDateTime = date.ToDateTime(TimeOnly.MinValue);
        var endDateTime = date.ToDateTime(TimeOnly.MaxValue);

        return await context.Bookings
            .AnyAsync(x => x.ServiceProviderId == guideId
                        && x.ServiceType == ServiceType.TourGuide
                        && x.StartDate < endDateTime
                        && x.EndDate > startDateTime
                        && x.BookingStatus != BookingStatus.Cancelled,
                      cancellationToken);
    }

    public async Task<List<Booking>> GetExpiredPendingBookingsAsync(CancellationToken cancellationToken)
    {
        var expiryTime = DateTimeOffset.UtcNow.AddMinutes(-30);

        return await context.Bookings
            .Where(x => x.PaymentStatus == PaymentTransactionStatus.Pending
                     && x.BookingStatus == BookingStatus.Pending
                     && x.CreatedAt <= expiryTime)
            .ToListAsync(cancellationToken);
    }
}
