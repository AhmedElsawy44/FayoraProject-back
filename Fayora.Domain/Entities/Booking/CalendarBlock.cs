using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Domain.Entities.Booking;

public class CalendarBlock : BaseEntity<Guid>
{
    public Guid ServiceId { get; init; }
    public ServiceType ServiceType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public BlockReason BlockReason { get; private set; }
    public Guid? BookingId { get; init; }
    public int ReservedSeats { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public CalendarBlock(Guid serviceId, ServiceType serviceType, DateTime startDate, DateTime endDate, BlockReason blockReason, Guid? bookingId = null, int reservedSeats = 1)
    {
        ServiceId = serviceId;
        ServiceType = serviceType;
        StartDate = startDate;
        EndDate = endDate;
        BlockReason = blockReason;
        BookingId = bookingId;
        ReservedSeats = reservedSeats;
    }

    private CalendarBlock() { }

    public void ConfirmBlock() => BlockReason = BlockReason.Booked;
}
