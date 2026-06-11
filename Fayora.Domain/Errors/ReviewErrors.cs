using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Errors;

public static class ReviewErrors
{
    public static readonly Error BookingNotFound = Error.NotFound(
        "Review.BookingNotFound",
        "The specified booking does not exist.");

    public static readonly Error BookingNotCompleted = Error.Validation(
        "Review.BookingNotCompleted",
        "Reviews can only be created for completed bookings.");

    public static readonly Error BookingNotOwnedByUser = Error.Forbidden(
        "Review.BookingNotOwnedByUser",
        "You do not have permission to review this booking.");

    public static readonly Error AlreadyReviewed = Error.Conflict(
        "Review.AlreadyReviewed",
        "This booking has already been reviewed.");

    public static readonly Error ReviewNotFound = Error.NotFound(
        "Review.NotFound",
        "The specified review does not exist.");

    public static readonly Error NotReviewOwner = Error.Forbidden(
        "Review.NotReviewOwner",
        "You are not the owner of this review.");

    public static readonly Error EditWindowExpired = Error.Validation(
        "Review.EditWindowExpired",
        "The 48-hour modification and deletion period has expired.");

    public static readonly Error AlreadyReported = Error.Conflict(
        "Review.AlreadyReported",
        "You have already reported this review.");

    public static readonly Error InvalidReportReason = Error.Validation(
        "Review.InvalidReportReason",
        "The report reason is invalid.");

    public static readonly Error InvalidTargetType = Error.Validation(
        "Review.InvalidTargetType",
        "The specified review target type is invalid.");

    public static readonly Error ReportNotFound = Error.NotFound(
        "Review.ReportNotFound",
        "The specified review report does not exist.");
}
