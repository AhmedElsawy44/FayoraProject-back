using Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;
using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageOccurrenceRepository
{
    Task<PackageOccurrence?> GetOccurrenceByPackageIdAndDate(Guid packageId, DateOnly date, CancellationToken cancellationToken);

    Task<PackageOccurrence?> GetByIdAsync(Guid occurrenceId, CancellationToken cancellationToken);

    Task ReleaseSeatsAsync(Guid packageId, DateOnly date, int count, CancellationToken cancellationToken);

    Task<List<CalendarBookingItemDto>> GetCalendarPackagessAsync(int year, int month, CancellationToken cancellationToken);
}
