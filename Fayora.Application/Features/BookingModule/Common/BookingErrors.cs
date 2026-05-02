using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Common;

public static class BookingErrors
{
    public static readonly Error OccurrenceNotFound = Error.NotFound(
        code: "Booking.OccurrenceNotFound",
        description: "The specified occurrence was not found.");
}
