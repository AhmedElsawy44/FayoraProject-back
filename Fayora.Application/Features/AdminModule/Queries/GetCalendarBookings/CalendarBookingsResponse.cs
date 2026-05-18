using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;

public record CalendarBookingsResponse(
    List<CalendarBookingItemDto> Items
);

public record CalendarBookingItemDto(
    Guid BookingId,
    string Title,
    DateOnly EventDate,
    string ServiceType 
);
