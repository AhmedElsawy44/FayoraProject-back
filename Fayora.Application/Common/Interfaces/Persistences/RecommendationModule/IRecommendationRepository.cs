using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TouristModule;

namespace Fayora.Application.Common.Interfaces.Persistences.RecommendationModule;

/// <summary>
/// Provides efficient, read-optimized data access for the recommendation engine.
/// All queries use AsNoTracking projections for maximum performance.
/// </summary>
public interface IRecommendationRepository
{
    /// <summary>
    /// Returns all active, approved packages with scoring metadata.
    /// </summary>
    Task<List<PackageScoringData>> GetCandidatePackagesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a user's interaction history (views, favorites) for the scoring window.
    /// </summary>
    Task<List<UserInteractionData>> GetUserInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the IDs of packages the user has booked.
    /// </summary>
    Task<List<Guid>> GetUserBookedPackageIdsAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Builds a co-occurrence map: for each package, which other packages
    /// were booked by users who also booked this one.
    /// Used for collaborative filtering (item-item).
    /// </summary>
    Task<Dictionary<Guid, HashSet<Guid>>> GetCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns global popularity stats (view count + booking count) per package
    /// within a rolling time window.
    /// </summary>
    Task<Dictionary<Guid, PopularityData>> GetPopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the interest IDs associated with a tourist profile.
    /// </summary>
    Task<List<int>> GetUserInterestIdsAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the user's budget tier and travel style for content-based scoring.
    /// </summary>
    Task<UserProfileData?> GetUserProfileDataAsync(
        Guid userId, CancellationToken cancellationToken);
}

// ── Projection DTOs ──────────────────────────────────────────────────────────

public record PackageScoringData(
    Guid Id,
    string Title,
    decimal AdultPrice,
    int DurationHours,
    string MainImageUrl,
    TourType TourTypes,
    int Views,
    DateTimeOffset CreatedAt,
    List<int> LocationIds);

public record UserInteractionData(
    Guid EntityId,
    InteractionType Type,
    DateTime CreatedAt);

public record PopularityData(
    int ViewCount,
    int BookingCount);

public record UserProfileData(
    Fayora.Domain.Enums.TouristModule.BudgetTier? BudgetTier,
    Fayora.Domain.Enums.TouristModule.TravelStyle? TravelStyle);
