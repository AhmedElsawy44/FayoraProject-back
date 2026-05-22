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

    /// <summary>
<<<<<<< HEAD
    /// Returns all active, approved housing units with scoring metadata.
    /// </summary>
    Task<List<HousingUnitScoringData>> GetCandidateUnitsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a user's unit interaction history (views, favorites) for the scoring window.
    /// </summary>
    Task<List<UserInteractionData>> GetUserUnitInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the IDs of housing units the user has booked.
    /// </summary>
    Task<List<Guid>> GetUserBookedUnitIdsAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Builds a co-occurrence map for housing units booking history.
    /// </summary>
    Task<Dictionary<Guid, HashSet<Guid>>> GetUnitCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns global popularity stats per unit within a rolling time window.
    /// </summary>
    Task<Dictionary<Guid, PopularityData>> GetUnitPopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all active guides with scoring metadata.
    /// </summary>
    Task<List<GuideScoringData>> GetCandidateGuidesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a user's guide interaction history (views, favorites) for the scoring window.
    /// </summary>
    Task<List<UserInteractionData>> GetUserGuideInteractionsAsync(
        Guid userId, int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the IDs of guides the user has booked.
    /// </summary>
    Task<List<Guid>> GetUserBookedGuideIdsAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Builds a co-occurrence map for guides booking history.
    /// </summary>
    Task<Dictionary<Guid, HashSet<Guid>>> GetGuideCoOccurrenceMapAsync(
        int daysWindow, CancellationToken cancellationToken);

    /// <summary>
    /// Returns global popularity stats per guide within a rolling time window.
    /// </summary>
    Task<Dictionary<Guid, PopularityData>> GetGuidePopularityStatsAsync(
        int daysWindow, CancellationToken cancellationToken);
=======
    /// Returns all locations with their associated active package IDs for scoring.
    /// </summary>
    Task<List<LocationScoringData>> GetCandidateLocationsAsync(CancellationToken cancellationToken);
>>>>>>> 5991f16 (recommded locations in home pageendpoint)
}

// ── Projection DTOs ──────────────────────────────────────────────────────────

public record LocationScoringData(
    int Id,
    string Name,
    string MainImageUrl,
    decimal Rating,
    LocationCategory Category,
    List<Guid> AssociatedPackageIds);

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

public record HousingUnitScoringData(
    Guid Id,
    string Title,
    decimal PricePerNight,
    string MainImageUrl,
    string AddressDetails,
    decimal Rating,
    int Views,
    DateTimeOffset CreatedAt,
    int LocationId,
    int MaxGuests,
    int BedRooms,
    int NumberOfBeds);

public record GuideScoringData(
    Guid UserId,
    string FullName,
    string? ProfileImageUrl,
    decimal? BaseRate,
    int? YearsOfExperience,
    bool IsSuperGuide,
    decimal AverageRating,
    int ReviewCount,
    int Views,
    DateTimeOffset CreatedAt,
    TourType TourTypes);

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

