using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;

public class GetCalendarBookingsQueryHandler(IPackageOccurrenceRepository packageOccurrenceRepository)
    : IQueryHandler<GetCalendarBookingsQuery, CalendarBookingsResponse>
{
    public async Task<CalendarBookingsResponse> Handle(
        GetCalendarBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await packageOccurrenceRepository.GetCalendarPackagessAsync(
            request.Year,
            request.Month,
            cancellationToken);


        return new CalendarBookingsResponse(items);
    }
}
