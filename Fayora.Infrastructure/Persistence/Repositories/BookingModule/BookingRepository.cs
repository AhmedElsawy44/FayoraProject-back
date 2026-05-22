using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;
using Fayora.Application.Features.AdminModule.Queries.GetTourGuidesStat;
using Fayora.Application.Features.AdminModule.Queries.GetTravelAgenciesStats;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Enums.BookingModule;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Fayora.Infrastructure.Persistence.Repositories.BookingModule;

public class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    public void AddBooking(Booking booking)
    {
        context.Bookings.Add(booking);
    }

    public Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken).AsTask();
    }

    public Task<bool> HasOverlapAsync(Guid serviceId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
    {
        return context.Bookings
            .AsNoTracking()
            .AnyAsync(
                b => b.ServiceId == serviceId
                     && b.BookingStatus != BookingStatus.Cancelled
                     && startDateTime < b.EndDate
                     && endDateTime > b.StartDate,
                cancellationToken);
    }

    // For guide booking
    public async Task<bool> HasGuideBookingOnDateAsync(
    Guid guideId,
    DateOnly date,
    CancellationToken cancellationToken)
    {
        var startDateTime = date.ToDateTime(TimeOnly.MinValue);
        var endDateTime = date.ToDateTime(TimeOnly.MaxValue);

        return await context.Bookings
            .AnyAsync(x => x.ServiceProviderId == guideId
                        && x.ServiceType == ServiceType.TourGuide
                        && x.StartDate < endDateTime
                        && x.EndDate > startDateTime
                        && x.BookingStatus != BookingStatus.Cancelled,
                      cancellationToken);
    }

    public async Task<List<Booking>> GetExpiredPendingBookingsAsync(CancellationToken cancellationToken)
    {
        var expiryTime = DateTimeOffset.UtcNow.AddMinutes(-30);

        return await context.Bookings
            .Where(x => x.PaymentStatus == PaymentTransactionStatus.Pending
                     && x.BookingStatus == BookingStatus.Pending
                     && x.CreatedAt <= expiryTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetPagedBookingsByUserIdAsync(
    Guid userId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public void RemoveBooking(Booking booking)
    {
        context.Bookings.Remove(booking);
    }

    public async Task<FinancialSummary> GetFinancialSummaryAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var query = context.Bookings
            .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate);

        if (!await query.AnyAsync(cancellationToken))
        {
            return new FinancialSummary(0, 0, 0, 0);
        }

        var grossRevenue = await query
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.TotalPrice, cancellationToken);

        var netProfit = await query
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.ServiceFee, cancellationToken);

        var pendingPayouts = await query
            .Where(b => b.PaymentStatus == PaymentTransactionStatus.Pending)
            .SumAsync(b => b.PayoutAmount, cancellationToken);

        int totalBookings = await query.CountAsync(cancellationToken);
        int refundedBookings = await query.CountAsync(b => b.BookingStatus == BookingStatus.Refunded, cancellationToken);

        double refundRate = totalBookings > 0
            ? Math.Round(((double)refundedBookings / totalBookings) * 100, 2)
            : 0;

        return new FinancialSummary(
            GrossRevenue: grossRevenue,
            NetProfit: netProfit,
            PendingPayouts: pendingPayouts,
            RefundRate: refundRate
        );
    }

    public async Task<List<MonthlyRevenueItem>> GetMonthlyRevenueItemsAsync(CancellationToken cancellationToken = default)
    {
        var chartStartDate = DateTime.UtcNow.AddMonths(-5);
        chartStartDate = new DateTime(chartStartDate.Year, chartStartDate.Month, 1);

        var bookings = await context.Bookings
            .Where(b => b.CreatedAt >= chartStartDate && b.CreatedAt <= DateTime.UtcNow)
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .Select(b => new { b.TotalPrice, b.PayoutAmount, b.CreatedAt })
            .ToListAsync(cancellationToken);

        var chartData = bookings
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .Select(g => new MonthlyRevenueItem(
                Month: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Revenue: g.Sum(x => x.TotalPrice),
                Payouts: g.Sum(x => x.PayoutAmount)
            ))
            .OrderBy(x => DateTime.ParseExact(x.Month, "MMM yyyy", CultureInfo.InvariantCulture))
            .ToList();

        return chartData;
    }

    public async Task<List<BookingMixItem>> GetBookingMixItemsAsync(CancellationToken cancellationToken = default)
    {
        var liveBookings = await context.Bookings
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .Select(b => new { b.ServiceType })
            .ToListAsync(cancellationToken);

        int totalLiveBookings = liveBookings.Count;

        if (totalLiveBookings == 0)
        {
            return [];
        }

        var mixData = liveBookings
            .GroupBy(b => b.ServiceType)
            .Select(g => new BookingMixItem(
                Category: g.Key.ToString(),
                Count: g.Count(),
                Percentage: Math.Round(((double)g.Count() / totalLiveBookings) * 100, 2)
            ))
            .OrderByDescending(x => x.Count)
            .ToList();

        return mixData;
    }

    public async Task<List<WeeklyBookingBarChartItem>> GetWeeklyBookingsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = today.AddDays(-diff);
        var endOfWeek = startOfWeek.AddDays(7);

        var bookingsThisWeek = await context.Bookings
            .Where(b => b.CreatedAt >= startOfWeek && b.CreatedAt < endOfWeek)
            .Select(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        var daysOfWeek = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        var chartData = daysOfWeek.Select((day, index) => new WeeklyBookingBarChartItem(
            Day: day,
            Count: bookingsThisWeek.Count(b => (int)b.DayOfWeek == (index == 6 ? 0 : index + 1))
        )).ToList();

        return chartData;
    }


    public async Task<int> GetOnTourNowCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .Where(b => b.ServiceType == ServiceType.TourGuide
                     && b.BookingStatus == BookingStatus.Completed
                        && b.StartDate <= DateTime.UtcNow && b.EndDate >= DateTime.UtcNow)
            .Select(b => b.ServiceProviderId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<GuideRevenueDto> GetGuideRevenueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow;

        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
        var nextMonth = firstDayOfCurrentMonth.AddMonths(1);

        var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);

        decimal currentMonthRevenue = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.TourGuide
                     && b.CreatedAt >= firstDayOfCurrentMonth
                     && b.CreatedAt < nextMonth)
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.PayoutAmount, cancellationToken);

        decimal lastMonthRevenue = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.TourGuide
                     && b.CreatedAt >= firstDayOfLastMonth
                     && b.CreatedAt < firstDayOfCurrentMonth)
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.PayoutAmount, cancellationToken);

        double growthPercentage = 0;
        if (lastMonthRevenue > 0)
        {
            growthPercentage = (double)((currentMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100;
            growthPercentage = Math.Round(growthPercentage, 2);
        }
        else if (currentMonthRevenue > 0)
        {
            growthPercentage = 100;
        }

        return new GuideRevenueDto(
            Amount: currentMonthRevenue,
            GrowthPercentage: growthPercentage
        );
    }

    public async Task<CombinedGmvDto> GetCompaniesGmvAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
        var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);

        // 1. إجمالي مبيعات الشهر الحالي (MTD) للشركات فقط
        decimal currentMonthGmv = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.GuidePackage
                     && b.CreatedAt >= firstDayOfCurrentMonth)
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.TotalPrice, cancellationToken);

        // 2. إجمالي مبيعات الشهر الماضي لنفس الشركات
        decimal lastMonthGmv = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.GuidePackage
                     && b.CreatedAt >= firstDayOfLastMonth
                     && b.CreatedAt < firstDayOfCurrentMonth)
            .Where(b => b.BookingStatus == BookingStatus.Completed)
            .SumAsync(b => b.TotalPrice, cancellationToken);

        double growthPercentage = 0;
        if (lastMonthGmv > 0)
        {
            growthPercentage = (double)((currentMonthGmv - lastMonthGmv) / lastMonthGmv) * 100;
        }
        else if (currentMonthGmv > 0)
        {
            growthPercentage = 100;
        }

        return new CombinedGmvDto(
            Amount: currentMonthGmv,
            GrowthPercentage: Math.Round(growthPercentage, 2)
        );
    }

    public async Task<AvgCommissionDto> GetCompaniesAvgCommissionAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
        var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);

        double? currentAvg = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.GuidePackage && b.CreatedAt >= firstDayOfCurrentMonth)
            .AverageAsync(b => (double?)b.ServiceFee, cancellationToken);

        double currentAvgCommission = currentAvg ?? 0.0;

        double? lastAvg = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.GuidePackage
                     && b.CreatedAt >= firstDayOfLastMonth
                     && b.CreatedAt < firstDayOfCurrentMonth)
            .AverageAsync(b => (double?)b.ServiceFee, cancellationToken);

        double lastAvgCommission = lastAvg ?? 0.0;

        double pointChange = currentAvgCommission - lastAvgCommission;

        return new AvgCommissionDto(
            Rate: Math.Round(currentAvgCommission, 2),
            PointChange: Math.Round(pointChange, 2)
        );
    }

    public async Task<double> GetLocationsAvgOccupancyAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);

        int totalHousingUnits = await context.HousingUnits.CountAsync(cancellationToken);

        if (totalHousingUnits == 0) return 0.0;

        int bookedUnitsCount = await context.Bookings
            .Where(b => b.ServiceType == ServiceType.Accommodation
                     && (b.BookingStatus == BookingStatus.Completed)
                     && b.CreatedAt >= firstDayOfCurrentMonth)
            .Select(b => b.SeatsCount)
            .Distinct()
            .CountAsync(cancellationToken);

        double occupancyRate = ((double)bookedUnitsCount / totalHousingUnits) * 100;

        return Math.Round(occupancyRate, 2);
    }
}
