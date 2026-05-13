using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.BookingModule.Commands.GetMyBookings;

public record GetMyBookingsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<List<BookingResponse>>;

public record BookingResponse(
    Guid BookingId,
    string Title,
    GeoPoint Location,
    DateTime Date,
    BookingStatus Status,
    string ImageUrl
);
