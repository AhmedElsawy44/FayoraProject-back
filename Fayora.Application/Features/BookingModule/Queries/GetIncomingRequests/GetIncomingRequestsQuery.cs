using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Enums.BookingModule;
using System;
using System.Collections.Generic;

namespace Fayora.Application.Features.BookingModule.Queries.GetIncomingRequests
{
    public record GetIncomingRequestsQuery(
        BookingStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<List<IncomingRequestResponse>>;

    public record IncomingRequestResponse(
        Guid BookingId,
        string TouristName,
        string TouristInitials,
        string ServiceTitle,
        DateTime StartDate,
        DateTime EndDate,
        int Nights,
        int GuestsCount,
        BookingStatus Status,
        decimal TotalPrice
    );
}
