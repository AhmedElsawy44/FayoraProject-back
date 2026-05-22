using Fayora.Application.Common.Interfaces.Persistences.RecommendationModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TouristModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.RecommendationModule;

/// <summary>
/// High-performance, read-optimized data access for the recommendation engine.
/// All queries use AsNoTracking + projections to avoid materializing full entity graphs.
/// </summary>
public class RecommendationRepository(ApplicationDbContext context) : IRecommendationRepository
{
    public async Task<List<PackageScoringData>> GetCandidatePackagesAsync(CancellationToken cancellationToken)
    {
        return await context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.IsActive
                        && p.PackageStatus == ItemStatus.Active
                        && p.DeletedAt == null)
            .Select(p => new PackageScoringData(
                p.Id,
                p.Title,
                p.AdultPrice,
                p.DurationHours,
                p.MainImageUrl.Value,
                p.TourTypes,
                p.Views,
                p.CreatedAt,
                p.LocationIds.ToList()))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInteractionData>> GetUserInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-daysWindow);

        return await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.UserId == userId
                        && i.EntityType == EntityType.Package
                        && i.CreatedAt >= cutoff)
            .Select(i => new UserInteractionData(
                i.EntityId,
                i.InteractionType,
                i.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetUserBookedPackageIdsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId
                        && b.ServiceType == ServiceType.GuidePackage)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, HashSet<Guid>>> GetCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        // Step 1: Get all package bookings within the window grouped by user
        var userBookings = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.GuidePackage
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.UserId)
            .Where(g => g.Count() >= 2) // Only users with 2+ bookings contribute
            .Select(g => new
            {
                UserId = g.Key,
                PackageIds = g.Select(b => b.ServiceId).Distinct().ToList()
            })
            .ToListAsync(cancellationToken);

        // Step 2: Build co-occurrence map (item-item collaborative filtering)
        var coOccurrence = new Dictionary<Guid, HashSet<Guid>>();

        foreach (var user in userBookings)
        {
            foreach (var packageId in user.PackageIds)
            {
                if (!coOccurrence.ContainsKey(packageId))
                    coOccurrence[packageId] = new HashSet<Guid>();

                foreach (var otherPackageId in user.PackageIds)
                {
                    if (otherPackageId != packageId)
                        coOccurrence[packageId].Add(otherPackageId);
                }
            }
        }

        return coOccurrence;
    }

    public async Task<Dictionary<Guid, PopularityData>> GetPopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        // View counts from UserInteractions
        var viewCounts = await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.EntityType == EntityType.Package
                        && i.InteractionType == InteractionType.View
                        && i.CreatedAt >= cutoff.DateTime)
            .GroupBy(i => i.EntityId)
            .Select(g => new { PackageId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PackageId, x => x.Count, cancellationToken);

        // Booking counts
        var bookingCounts = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.GuidePackage
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.ServiceId)
            .Select(g => new { PackageId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PackageId, x => x.Count, cancellationToken);

        // Merge into PopularityData
        var allIds = viewCounts.Keys.Union(bookingCounts.Keys).Distinct();
        var result = new Dictionary<Guid, PopularityData>();

        foreach (var id in allIds)
        {
            viewCounts.TryGetValue(id, out var views);
            bookingCounts.TryGetValue(id, out var bookings);
            result[id] = new PopularityData(views, bookings);
        }

        return result;
    }

    public async Task<List<int>> GetUserInterestIdsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        var profile = await context.Tourists
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return profile?.Interests.ToList() ?? new List<int>();
    }

    public async Task<UserProfileData?> GetUserProfileDataAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        return await context.Tourists
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => new UserProfileData(t.BudgetTier, t.TravelStyle))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
