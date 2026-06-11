using Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;
using Fayora.Application.Features.AdminModule.Queries.GetTourGuidesStat;
using Fayora.Application.Features.AdminModule.Queries.GetTravelAgenciesStats;
using Fayora.Domain.Entities.Booking;

namespace Fayora.Application.Common.Interfaces.Persistences.BookingModule;

public interface IBookingRepository
{
    void AddBooking(Booking booking);
    Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid serviceId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);
    Task<bool> HasBookingsForPackageAsync(Guid packageId, CancellationToken cancellationToken = default);

    Task<bool> HasBookingsForOccurrenceAsync(Guid packageId, DateOnly date, CancellationToken cancellationToken = default);

    void RemoveBooking(Booking booking);

    //for expired pending bookings
    Task<List<Booking>> GetExpiredPendingBookingsAsync(CancellationToken cancellationToken = default);

    //for guide booking
    Task<bool> HasGuideBookingOnDateAsync(
    Guid guideId,
    DateOnly date,
    CancellationToken cancellationToken);
    Task<List<Booking>> GetPagedBookingsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<FinancialSummary> GetFinancialSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<MonthlyRevenueItem>> GetMonthlyRevenueItemsAsync(CancellationToken cancellationToken = default);
    Task<List<BookingMixItem>> GetBookingMixItemsAsync(CancellationToken cancellationToken = default);
    Task<List<WeeklyBookingBarChartItem>> GetWeeklyBookingsAsync(CancellationToken cancellationToken = default);
    Task<int> GetOnTourNowCountAsync(CancellationToken cancellationToken = default);
    Task<GuideRevenueDto> GetGuideRevenueAsync(CancellationToken cancellationToken = default);
    Task<CombinedGmvDto> GetCompaniesGmvAsync(CancellationToken cancellationToken);
    Task<AvgCommissionDto> GetCompaniesAvgCommissionAsync(CancellationToken cancellationToken);
    Task<double> GetLocationsAvgOccupancyAsync(CancellationToken cancellationToken = default);

    Task<List<Booking>> GetBookingsForOccurrenceAsync(
        Guid packageId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
