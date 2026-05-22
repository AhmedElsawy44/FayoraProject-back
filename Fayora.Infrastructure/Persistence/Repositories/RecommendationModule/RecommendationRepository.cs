using Fayora.Application.Common.Interfaces.Persistences.RecommendationModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.Enums.TouristModule;
using Fayora.Domain.Enums.SharedModule;
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

    public async Task<List<HousingUnitScoringData>> GetCandidateUnitsAsync(CancellationToken cancellationToken)
    {
        return await context.HousingUnits
            .AsNoTracking()
            .Where(u => u.Status == ItemStatus.Active)
            .Select(u => new HousingUnitScoringData(
                u.Id,
                u.Title,
                u.PricePerNight,
                u.MainImageUrl.Value,
                u.AddressDetails,
                u.Rating,
                u.Views,
                u.CreatedAt,
                u.LocationId,
                u.MaxGuests,
                u.BedRooms,
                u.NumberOfBeds))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInteractionData>> GetUserUnitInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-daysWindow);

        return await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.UserId == userId
                        && i.EntityType == EntityType.Accommodation
                        && i.CreatedAt >= cutoff)
            .Select(i => new UserInteractionData(
                i.EntityId,
                i.InteractionType,
                i.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetUserBookedUnitIdsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId
                        && b.ServiceType == ServiceType.Accommodation)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, HashSet<Guid>>> GetUnitCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        var userBookings = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.Accommodation
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.UserId)
            .Where(g => g.Count() >= 2)
            .Select(g => new
            {
                UserId = g.Key,
                UnitIds = g.Select(b => b.ServiceId).Distinct().ToList()
            })
            .ToListAsync(cancellationToken);

        var coOccurrence = new Dictionary<Guid, HashSet<Guid>>();

        foreach (var user in userBookings)
        {
            foreach (var unitId in user.UnitIds)
            {
                if (!coOccurrence.ContainsKey(unitId))
                    coOccurrence[unitId] = new HashSet<Guid>();

                foreach (var otherUnitId in user.UnitIds)
                {
                    if (otherUnitId != unitId)
                        coOccurrence[unitId].Add(otherUnitId);
                }
            }
        }

        return coOccurrence;
    }

    public async Task<Dictionary<Guid, PopularityData>> GetUnitPopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        var viewCounts = await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.EntityType == EntityType.Accommodation
                        && i.InteractionType == InteractionType.View
                        && i.CreatedAt >= cutoff.DateTime)
            .GroupBy(i => i.EntityId)
            .Select(g => new { UnitId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UnitId, x => x.Count, cancellationToken);

        var bookingCounts = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.Accommodation
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.ServiceId)
            .Select(g => new { UnitId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UnitId, x => x.Count, cancellationToken);

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

    public async Task<List<GuideScoringData>> GetCandidateGuidesAsync(CancellationToken cancellationToken)
    {
        var guides = await context.TourGuides
            .AsNoTracking()
            .Where(g => g.Status == ItemStatus.Active)
            .Join(context.Users.AsNoTracking(),
                  g => g.UserId,
                  u => u.Id,
                  (g, u) => new
                  {
                      g.UserId,
                      FullName = u.FirstName + " " + u.LastName,
                      ProfileImageUrl = u.ProfileImageUrl != null ? u.ProfileImageUrl.Value : null,
                      g.BaseRate,
                      g.YearsOfExperience,
                      g.IsSuperGuide,
                      g.AverageRating,
                      g.ReviewCount,
                      g.Views,
                      g.CreatedAt
                  })
            .ToListAsync(cancellationToken);

        // Fetch package tour types for all guides to aggregate in-memory
        var guidePackages = await context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.IsActive && p.PackageStatus == ItemStatus.Active && p.DeletedAt == null)
            .Select(p => new { p.UserId, p.TourTypes })
            .ToListAsync(cancellationToken);

        var packageMap = guidePackages
            .GroupBy(p => p.UserId)
            .ToDictionary(g => g.Key, g => g.Select(p => p.TourTypes).ToList());

        var candidates = new List<GuideScoringData>();
        foreach (var item in guides)
        {
            TourType combinedTypes = TourType.None;
            if (packageMap.TryGetValue(item.UserId, out var typesList))
            {
                foreach (var t in typesList)
                {
                    combinedTypes |= t;
                }
            }

            candidates.Add(new GuideScoringData(
                item.UserId,
                item.FullName,
                item.ProfileImageUrl,
                item.BaseRate,
                item.YearsOfExperience,
                item.IsSuperGuide,
                item.AverageRating,
                item.ReviewCount,
                item.Views,
                item.CreatedAt,
                combinedTypes));
        }

        return candidates;
    }

    public async Task<List<UserInteractionData>> GetUserGuideInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-daysWindow);

        return await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.UserId == userId
                        && i.EntityType == EntityType.TourGuide
                        && i.CreatedAt >= cutoff)
            .Select(i => new UserInteractionData(
                i.EntityId,
                i.InteractionType,
                i.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetUserBookedGuideIdsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId
                        && b.ServiceType == ServiceType.TourGuide)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, HashSet<Guid>>> GetGuideCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        var userBookings = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.TourGuide
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.UserId)
            .Where(g => g.Count() >= 2)
            .Select(g => new
            {
                UserId = g.Key,
                GuideIds = g.Select(b => b.ServiceId).Distinct().ToList()
            })
            .ToListAsync(cancellationToken);

        var coOccurrence = new Dictionary<Guid, HashSet<Guid>>();

        foreach (var user in userBookings)
        {
            foreach (var guideId in user.GuideIds)
            {
                if (!coOccurrence.ContainsKey(guideId))
                    coOccurrence[guideId] = new HashSet<Guid>();

                foreach (var otherGuideId in user.GuideIds)
                {
                    if (otherGuideId != guideId)
                        coOccurrence[guideId].Add(otherGuideId);
                }
            }
        }

        return coOccurrence;
    }

    public async Task<Dictionary<Guid, PopularityData>> GetGuidePopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-daysWindow);

        var viewCounts = await context.UserInteractions
            .AsNoTracking()
            .Where(i => i.EntityType == EntityType.TourGuide
                        && i.InteractionType == InteractionType.View
                        && i.CreatedAt >= cutoff.DateTime)
            .GroupBy(i => i.EntityId)
            .Select(g => new { GuideId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GuideId, x => x.Count, cancellationToken);

        var bookingCounts = await context.Bookings
            .AsNoTracking()
            .Where(b => b.ServiceType == ServiceType.TourGuide
                        && b.CreatedAt >= cutoff
                        && b.BookingStatus != Domain.Enums.BookingModule.BookingStatus.Cancelled)
            .GroupBy(b => b.ServiceId)
            .Select(g => new { GuideId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GuideId, x => x.Count, cancellationToken);

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
}
