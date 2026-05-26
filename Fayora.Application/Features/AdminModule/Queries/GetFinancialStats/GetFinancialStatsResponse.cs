namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public record FinancialStatsResponse(
    FinancialSummary Summary,
    List<MonthlyRevenueItem> RevenueChart,
    List<BookingMixItem> BookingMix,
    List<WeeklyBookingBarChartItem> WeeklyBookingBarChartItems
);

public record FinancialSummary(
    decimal GrossRevenue,
    decimal NetProfit,
    decimal PendingPayouts,
    double RefundRate
);

public record MonthlyRevenueItem(string Month, decimal Revenue, decimal Payouts);
public record BookingMixItem(string Category, int Count, double Percentage);

public record WeeklyBookingBarChartItem(
    string Day,
    int Count
);
