using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public class GetFinancialStatsHandler(IBookingRepository bookingRepository)
    : IQueryHandler<GetFinancialStatsQuery, FinancialStatsResponse>
{
    public async Task<FinancialStatsResponse> Handle(
        GetFinancialStatsQuery request,
        CancellationToken cancellationToken)
    {
        var summary = await bookingRepository.GetFinancialSummaryAsync(request.StartDate, request.EndDate, cancellationToken);

        var monthlyRevueItems = await bookingRepository.GetMonthlyRevenueItemsAsync(cancellationToken);

        var bookingMixItems = await bookingRepository.GetBookingMixItemsAsync(cancellationToken);

        var weeklyBookingBarChartItems = await bookingRepository.GetWeeklyBookingsAsync(cancellationToken);

        return new FinancialStatsResponse(summary, monthlyRevueItems, bookingMixItems, weeklyBookingBarChartItems);
    }
}
