using Fayora.Domain.Common.Entity.Constants;
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
    public bool IsScanned { get; private set; }
    public DateTimeOffset? ScannedAt { get; private set; }

    public bool IsCashOnArrival { get; private set; }
    public decimal DepositAmount { get; private set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;


    public static Result<Booking> Create(
        Guid userId, Guid providerId, ServiceType type, Guid serviceId,
        decimal basePrice, decimal serviceFee, decimal payoutAmount,
        int seatsCount, CancellationPolicy policy, DateTime startDate,
        DateTime endDate, bool isCashOnArrival = false)
    {
        if (endDate <= startDate)
            return Error.Validation();

        decimal depositAmount = isCashOnArrival ? basePrice * BookingConstants.CashOnArrivalDepositRate : 0;

        var booking = new Booking
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
            EndDate = endDate,
            IsCashOnArrival = isCashOnArrival,
            DepositAmount = depositAmount
        };

        booking.RaiseDomainEvent(new BookingCreatedEvent(booking.Id));

        return booking;
    }

    public Result<Success> MarkAsPaid()
    {
        if (PaymentStatus == PaymentTransactionStatus.Paid)
            return Error.Validation("Booking is already paid.");

        PaymentStatus = PaymentTransactionStatus.Paid;
        BookingStatus = BookingStatus.Completed;

        RaiseDomainEvent(new BookingPaidEvent(Id));

        return Result.Success;
    }


    public Result<Success> MarkDepositAsPaid()
    {
        if (!IsCashOnArrival)
            return Error.Validation("Booking.NotCashOnArrival", "This booking is not cash on arrival.");

        if (PaymentStatus == PaymentTransactionStatus.PartiallyPaid)
            return Error.Validation("Booking.DepositAlreadyPaid", "Deposit is already paid.");

        PaymentStatus = PaymentTransactionStatus.PartiallyPaid;

        RaiseDomainEvent(new BookingDepositPaidEvent(Id));

        return Result.Success;
    }
    public Result<Success> MarkAsRefunded()
    {
        if (BookingStatus == BookingStatus.Refunded)
            return Error.Validation("Booking.AlreadyRefunded", "Booking is already refunded.");

        BookingStatus = BookingStatus.Refunded;
        PaymentStatus = PaymentTransactionStatus.Refunded;
        return Result.Success;
    }

    public Result<Success> ConfirmCashReceived()
    {
        if (!IsCashOnArrival)
            return Error.Validation("Booking.NotCashOnArrival", "This booking is not cash on arrival.");

        if (BookingStatus == BookingStatus.Completed)
            return Error.Validation("Booking.AlreadyCompleted", "Booking is already completed.");

        if (!IsScanned)
            return Error.Validation("Booking.NotScanned", "QR code must be scanned first.");

        BookingStatus = BookingStatus.Completed;
        PaymentStatus = PaymentTransactionStatus.Paid;

        RaiseDomainEvent(new BookingCompletedEvent(Id));

        return Result.Success;
    }

    public Result<Success> MarkAsScanned()
    {
        if (IsScanned)
            return Error.Conflict("QR code already used.");

        IsScanned = true;
        ScannedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    // we need to handle it by domain event to apply cancellation policy and calculate refund amount
    public Result<Success> Cancel(string reason)
    {
        if (BookingStatus == BookingStatus.Completed)
            return Error.Validation("Cannot cancel a completed booking.");

        if (BookingStatus == BookingStatus.Cancelled)
            return Error.Validation("Booking is already cancelled.");


        if (AppliedCancelPolicy == CancellationPolicy.FreeCancellation48Hours)
        {
            var hoursUntilStart = (StartDate - DateTime.UtcNow).TotalHours;
            if (hoursUntilStart < 48)
                return Error.Validation("Booking.CancellationWindowPassed",
                    "Cannot cancel. Cancellation is only allowed 48 hours before the trip.");
        }
        else if (AppliedCancelPolicy == CancellationPolicy.NonRefundable)
        {
            return Error.Validation("Booking.NonRefundable",
                "This booking is non-refundable and cannot be cancelled.");
        }

        BookingStatus = BookingStatus.Cancelled;
        RaiseDomainEvent(new BookingCanceledEvent(Id));

        return Result.Success;
    }

    private Booking() { }
}
