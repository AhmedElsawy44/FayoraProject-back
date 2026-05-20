using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Infrastructure.Jobs
{
    public class ExpiredBookingsJob(
        IBookingRepository bookingRepository,
        ICalendarBlockRepository calendarBlockRepository,
        IPackageOccurrenceRepository packageOccurrenceRepository,
        IUnitOfWork unitOfWork)
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var expiredBookings = await bookingRepository
                .GetExpiredPendingBookingsAsync(cancellationToken);

            foreach (var booking in expiredBookings)
            {

                // if it's cash on arrival and only partially paid, skip cancellation to allow them to complete payment at the counter
                if (booking.IsCashOnArrival &&
                    booking.PaymentStatus == PaymentTransactionStatus.PartiallyPaid)
                    continue;

                // cancel the booking
                booking.Cancel("Payment timeout");

                // remove the associated CalendarBlock
                await calendarBlockRepository
                    .RemoveByBookingIdAsync(booking.Id, cancellationToken);

                // if it's a package, release the seats
                if (booking.ServiceType == ServiceType.GuidePackage)
                {
                    await packageOccurrenceRepository
                        .ReleaseSeatsAsync(booking.ServiceId,
                                          DateOnly.FromDateTime(booking.StartDate),
                                          booking.SeatsCount,
                                          cancellationToken);
                }
            }

            await unitOfWork.CommitChangesAsync(cancellationToken);
        }
    }
}
