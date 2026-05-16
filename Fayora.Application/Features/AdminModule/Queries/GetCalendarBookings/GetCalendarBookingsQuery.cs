using Fayora.Application.Common.Abstractions.Messaging;

namespace Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;

public record GetCalendarBookingsQuery(
    int Year,
    int Month
) : IQuery<CalendarBookingsResponse>;
