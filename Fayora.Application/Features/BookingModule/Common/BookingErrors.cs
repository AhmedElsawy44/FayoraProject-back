using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Common;

public static class BookingErrors
{
    public static readonly Error OccurrenceNotFound = Error.NotFound(
        code: "Booking.OccurrenceNotFound",
        description: "The specified occurrence was not found."
    );

    public static readonly Error InvalidPaymentWebhook = Error.Validation(
        code: "Booking.InvalidPaymentWebhook",
        description: "The payment webhook data is invalid or failed validation."
    );

    public static readonly Error InvalidBookingId = Error.Validation(
        code: "Booking.InvalidBookingId",
        description: "The booking ID provided in the payment webhook is invalid."
    );

    public static readonly Error BookingNotFound = Error.NotFound(
        code: "Booking.BookingNotFound",
        description: "The booking associated with the payment webhook was not found."
    );

    public static readonly Error PaymentTransactionNotFound = Error.NotFound(
        code: "Booking.PaymentTransactionNotFound",
        description: "The payment transaction associated with the payment webhook was not found."
    );

    public static readonly Error InvalidQrToken = Error.Validation(
    "Booking.InvalidQrToken",
    "The QR token is invalid or expired."
);

    public static readonly Error UnauthorizedScan = Error.Unauthorized(
        "Booking.UnauthorizedScan",
        "You are not authorized to scan this QR code."
    );

    public static readonly Error BookingNotPaid = Error.Validation(
        "Booking.NotPaid",
        "Booking must be paid before generating QR code."
    );

    public static readonly Error Unauthorized = Error.Unauthorized(
        "Booking.Unauthorized",
        "You are not authorized to access this booking."
    );

    public static readonly Error BookingCancelled = Error.Validation(
    "Booking.Cancelled",
    "This booking has been cancelled."
);
}
