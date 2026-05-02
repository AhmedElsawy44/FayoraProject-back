using Fayora.Domain.Common.Events.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Domain.Entities.Booking;

public class Booking : BaseEntity<Guid>
{
    public Guid UserId { get; init; }
    public Guid ServiceProviderId { get; init; }
    public Guid ServiceId { get; init; }
    public ServiceType ServiceType { get; init; }
    public decimal BasePrice { get; init; }
    public decimal ServiceFee { get; init; }
    public decimal PayoutAmount { get; init; }
    public decimal TotalPrice { get; init; }
    public int SeatsCount { get; init; }
    public CancellationPolicy AppliedCancelPolicy { get; init; }
    public BookingStatus BookingStatus { get; private set; }
    public PaymentTransactionStatus PaymentStatus { get; private set; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public static Result<Booking> Create(
        Guid userId, Guid providerId, ServiceType type, Guid serviceId,
        decimal basePrice, decimal serviceFee, decimal payoutAmount,
        int seatsCount, CancellationPolicy policy, DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            return Error.Validation();

        return new Booking
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            ServiceProviderId = providerId,
            ServiceType = type,
            ServiceId = serviceId,
            BasePrice = basePrice,
            ServiceFee = serviceFee,
            TotalPrice = basePrice + serviceFee,
            PayoutAmount = payoutAmount,
            SeatsCount = seatsCount,
            AppliedCancelPolicy = policy,
            BookingStatus = BookingStatus.Pending,
            PaymentStatus = PaymentTransactionStatus.Pending,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    public Result<Success> MarkAsPaid()
    {
        if (PaymentStatus == PaymentTransactionStatus.Paid)
            return Error.Validation("Booking is already paid.");

        PaymentStatus = PaymentTransactionStatus.Paid;
        BookingStatus = BookingStatus.Completed;
        return Result.Success;
    }

    public Result<Success> Cancel(string reason)
    {
        if (BookingStatus == BookingStatus.Completed)
            return Error.Validation("Cannot cancel a completed booking.");

        BookingStatus = BookingStatus.Cancelled;

        RaiseDomainEvent(new BookingCanceledEvent(Id));

        return Result.Success;
    }

    private Booking() { }
}
