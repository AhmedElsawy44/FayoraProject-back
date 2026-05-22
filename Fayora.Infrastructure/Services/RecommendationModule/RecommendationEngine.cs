using Fayora.Application.Common.Abstractions.Caching;
using Fayora.Application.Common.Interfaces.Persistences.RecommendationModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedLocations;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TouristModule;

using Microsoft.Extensions.Logging;

namespace Fayora.Infrastructure.Services.RecommendationModule;

/// <summary>
/// Multi-signal recommendation engine combining:
/// 1. Content-Based Filtering  (user interests ↔ package TourTypes + budget match)
/// 2. Collaborative Filtering   (item-item co-occurrence from booking history)
/// 3. Popularity Scoring        (views + bookings in rolling window)
/// 4. Recency Boost             (exponential decay favoring newer packages)
///
/// Cold-start fallback: trending packages with diversity injection.
/// Also supports location recommendations by aggregating package scores per location.
/// </summary>
public class RecommendationEngine(
    IRecommendationRepository repository,
    ICacheService cache,
    ILogger<RecommendationEngine> logger) : IRecommendationService
{
    // ── Scoring Weights ──────────────────────────────────────────────────────
    private const double W_Content = 0.35;
    private const double W_Collaborative = 0.30;
    private const double W_Popularity = 0.20;
    private const double W_Recency = 0.15;

    // ── Configuration ────────────────────────────────────────────────────────
    private const int ScoringWindowDays = 30;
    private const int RecencyHalfLifeDays = 14;
    private const int MinDiversityCategories = 3;
    private const string CoOccurrenceCacheKey = "recommendation:co-occurrence";
    private static readonly TimeSpan CoOccurrenceCacheTtl = TimeSpan.FromHours(1);

    // ── Interest → TourType Mapping ──────────────────────────────────────────
    // Seeded interests: 1=Nature, 2=Historical, 3=Cultural, 4=Adventure, 5=Camping, 6=Wildlife
    private static readonly Dictionary<int, TourType> InterestToTourTypeMap = new()
    {
        { 1, TourType.Nature },
        { 2, TourType.Historical },
        { 3, TourType.Cultural },
        { 4, TourType.Adventure },
        { 5, TourType.Nature | TourType.Adventure },  // Camping → Nature + Adventure
        { 6, TourType.Nature },                         // Wildlife → Nature
    };

    // ── Budget Tier → Price Range ────────────────────────────────────────────
    private static readonly Dictionary<BudgetTier, (decimal Min, decimal Max)> BudgetPriceRanges = new()
    {
        { BudgetTier.Low, (0m, 500m) },
        { BudgetTier.Medium, (200m, 2000m) },
        { BudgetTier.Luxury, (1000m, decimal.MaxValue) },
    };

    // ═════════════════════════════════════════════════════════════════════════
    //  PUBLIC API — PACKAGES
    // ═════════════════════════════════════════════════════════════════════════

    public async Task<List<RecommendedPackageResult>> GetPersonalizedAsync(
        Guid userId, int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating personalized recommendations for user {UserId}", userId);

        // 1. Load all data sequentially
        var candidates = await repository.GetCandidatePackagesAsync(cancellationToken);
        var userInterests = await repository.GetUserInterestIdsAsync(userId, cancellationToken);
        var userProfile = await repository.GetUserProfileDataAsync(userId, cancellationToken);
        var interactions = await repository.GetUserInteractionsAsync(userId, ScoringWindowDays, cancellationToken);
        var bookedIds = (await repository.GetUserBookedPackageIdsAsync(userId, cancellationToken)).ToHashSet();
        var popularity = await repository.GetPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        // 2. Load co-occurrence map (cached)
        var coOccurrence = await GetCachedCoOccurrenceAsync(cancellationToken);

        // 3. Compute user-specific signals
        var userTourTypes = GetUserTourTypePreferences(userInterests);
        var favoritedIds = interactions
            .Where(i => i.Type == InteractionType.Favorite)
            .Select(i => i.EntityId)
            .ToHashSet();
        var viewedIds = interactions
            .Where(i => i.Type == InteractionType.View)
            .Select(i => i.EntityId)
            .ToHashSet();

        // 4. Find collaborative candidates (packages co-booked with user's bookings)
        var collaborativeCandidates = GetCollaborativeCandidates(bookedIds, coOccurrence);

        // 5. Compute normalization denominators
        var maxViews = candidates.Count > 0 ? candidates.Max(c => c.Views) : 1;
        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        // 6. Score each candidate
        var scoredPackages = new List<(PackageScoringData Package, double Score, string Reason)>();

        foreach (var pkg in candidates)
        {
            // Skip already-booked packages
            if (bookedIds.Contains(pkg.Id))
                continue;

            var (contentScore, contentReason) = ScoreContentBased(pkg, userTourTypes, userProfile);
            var collaborativeScore = ScoreCollaborative(pkg.Id, collaborativeCandidates);
            var popularityScore = ScorePopularity(pkg.Id, popularity, maxPopularity);
            var recencyScore = ScoreRecency(pkg.CreatedAt);

            // Boost for favorited but not yet booked
            double favoriteBoost = favoritedIds.Contains(pkg.Id) ? 0.10 : 0.0;
            // Small boost for viewed (signal of existing interest)
            double viewBoost = viewedIds.Contains(pkg.Id) ? 0.03 : 0.0;

            var finalScore =
                (W_Content * contentScore) +
                (W_Collaborative * collaborativeScore) +
                (W_Popularity * popularityScore) +
                (W_Recency * recencyScore) +
                favoriteBoost + viewBoost;

            // Determine primary recommendation reason
            var reason = DetermineReason(contentScore, collaborativeScore, popularityScore, contentReason);

            scoredPackages.Add((pkg, finalScore, reason));
        }

        // 7. Sort and take top N, ensuring diversity
        var results = ApplyDiversityAndSelect(scoredPackages, count);

        logger.LogInformation("Generated {Count} personalized recommendations for user {UserId}", results.Count, userId);

        return results;
    }

    public async Task<List<RecommendedPackageResult>> GetTrendingAsync(
        int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating trending recommendations (anonymous/cold-start)");

        var candidates = await repository.GetCandidatePackagesAsync(cancellationToken);
        var popularity = await repository.GetPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        var scored = candidates.Select(pkg =>
        {
            var popScore = ScorePopularity(pkg.Id, popularity, maxPopularity);
            var recScore = ScoreRecency(pkg.CreatedAt);
            var finalScore = (0.65 * popScore) + (0.35 * recScore);

            return (Package: pkg, Score: finalScore, Reason: "🔥 Trending in Fayoum");
        }).ToList();

        var results = ApplyDiversityAndSelect(scored, count);

        logger.LogInformation("Generated {Count} trending recommendations", results.Count);

        return results;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  PUBLIC API — LOCATIONS
    // ═════════════════════════════════════════════════════════════════════════

    public async Task<List<RecommendedLocationResult>> GetPersonalizedLocationsAsync(
        Guid userId, int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating personalized location recommendations for user {UserId}", userId);

        // 1. Load locations and packages sequentially
        var locations = await repository.GetCandidateLocationsAsync(cancellationToken);
        var candidates = await repository.GetCandidatePackagesAsync(cancellationToken);
        var userInterests = await repository.GetUserInterestIdsAsync(userId, cancellationToken);
        var userProfile = await repository.GetUserProfileDataAsync(userId, cancellationToken);
        var interactions = await repository.GetUserInteractionsAsync(userId, ScoringWindowDays, cancellationToken);
        var bookedIds = (await repository.GetUserBookedPackageIdsAsync(userId, cancellationToken)).ToHashSet();
        var popularity = await repository.GetPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        // 2. Load co-occurrence map (cached)
        var coOccurrence = await GetCachedCoOccurrenceAsync(cancellationToken);

        // 3. Compute user-specific signals
        var userTourTypes = GetUserTourTypePreferences(userInterests);
        var favoritedIds = interactions
            .Where(i => i.Type == InteractionType.Favorite)
            .Select(i => i.EntityId)
            .ToHashSet();
        var viewedIds = interactions
            .Where(i => i.Type == InteractionType.View)
            .Select(i => i.EntityId)
            .ToHashSet();

        var collaborativeCandidates = GetCollaborativeCandidates(bookedIds, coOccurrence);

        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        // 4. Score ALL packages (build a lookup: PackageId → Score + Reason)
        var packageScores = new Dictionary<Guid, (double Score, string Reason)>();
        foreach (var pkg in candidates)
        {
            var (contentScore, contentReason) = ScoreContentBased(pkg, userTourTypes, userProfile);
            var collaborativeScore = ScoreCollaborative(pkg.Id, collaborativeCandidates);
            var popularityScore = ScorePopularity(pkg.Id, popularity, maxPopularity);
            var recencyScore = ScoreRecency(pkg.CreatedAt);

            double favoriteBoost = favoritedIds.Contains(pkg.Id) ? 0.10 : 0.0;
            double viewBoost = viewedIds.Contains(pkg.Id) ? 0.03 : 0.0;

            var finalScore =
                (W_Content * contentScore) +
                (W_Collaborative * collaborativeScore) +
                (W_Popularity * popularityScore) +
                (W_Recency * recencyScore) +
                favoriteBoost + viewBoost;

            var reason = DetermineReason(contentScore, collaborativeScore, popularityScore, contentReason);
            packageScores[pkg.Id] = (finalScore, reason);
        }

        // 5. Aggregate package scores per location
        var results = AggregateLocationScores(locations, packageScores, count, isPersonalized: true);

        logger.LogInformation("Generated {Count} personalized location recommendations for user {UserId}",
            results.Count, userId);

        return results;
    }

    public async Task<List<RecommendedLocationResult>> GetTrendingLocationsAsync(
        int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating trending location recommendations (anonymous/cold-start)");

        var locations = await repository.GetCandidateLocationsAsync(cancellationToken);
        var candidates = await repository.GetCandidatePackagesAsync(cancellationToken);
        var popularity = await repository.GetPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        // Score all packages with trending formula
        var packageScores = new Dictionary<Guid, (double Score, string Reason)>();
        foreach (var pkg in candidates)
        {
            var popScore = ScorePopularity(pkg.Id, popularity, maxPopularity);
            var recScore = ScoreRecency(pkg.CreatedAt);
            var finalScore = (0.65 * popScore) + (0.35 * recScore);

            packageScores[pkg.Id] = (finalScore, "🔥 Trending in Fayoum");
        }

        // Aggregate per location
        var results = AggregateLocationScores(locations, packageScores, count, isPersonalized: false);

        logger.LogInformation("Generated {Count} trending location recommendations", results.Count);

        return results;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  SCORING FUNCTIONS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Content-Based: Jaccard similarity of user TourType preferences vs package TourTypes
    /// + budget-tier price band match.
    /// Returns a score in [0, 1] and a human-readable reason.
    /// </summary>
    private static (double Score, string Reason) ScoreContentBased(
        PackageScoringData package,
        HashSet<TourType> userTourTypes,
        UserProfileData? profile)
    {
        if (userTourTypes.Count == 0 && profile is null)
            return (0.0, string.Empty);

        double tourTypeScore = 0.0;
        string matchedType = string.Empty;

        if (userTourTypes.Count > 0)
        {
            // Compute Jaccard similarity between user interests and package tour types
            var packageTypes = DecomposeTourTypes(package.TourTypes);
            var intersection = userTourTypes.Intersect(packageTypes).Count();
            var union = userTourTypes.Union(packageTypes).Count();

            tourTypeScore = union > 0 ? (double)intersection / union : 0.0;

            if (intersection > 0)
            {
                matchedType = userTourTypes.Intersect(packageTypes).First().ToString();
            }
        }

        double budgetScore = 0.0;
        if (profile?.BudgetTier is not null && BudgetPriceRanges.TryGetValue(profile.BudgetTier.Value, out var range))
        {
            if (package.AdultPrice >= range.Min && package.AdultPrice <= range.Max)
                budgetScore = 1.0;
            else
            {
                // Partial credit: how far outside the range
                var distance = package.AdultPrice < range.Min
                    ? (double)(range.Min - package.AdultPrice) / (double)range.Min
                    : (double)(package.AdultPrice - range.Max) / (double)package.AdultPrice;
                budgetScore = Math.Max(0.0, 1.0 - distance);
            }
        }

        // Weighted combination: 70% tour type match, 30% budget match
        var finalScore = (0.7 * tourTypeScore) + (0.3 * budgetScore);

        var reason = !string.IsNullOrEmpty(matchedType)
            ? $"✨ Based on your interest in {matchedType} tours"
            : budgetScore > 0.5
                ? "💰 Matches your budget preferences"
                : string.Empty;

        return (finalScore, reason);
    }

    /// <summary>
    /// Collaborative: How strongly this package is connected to the user's booked packages
    /// through co-occurrence. Returns a score in [0, 1].
    /// </summary>
    private static double ScoreCollaborative(
        Guid packageId,
        Dictionary<Guid, int> collaborativeCandidates)
    {
        if (collaborativeCandidates.Count == 0)
            return 0.0;

        if (!collaborativeCandidates.TryGetValue(packageId, out var coCount))
            return 0.0;

        var maxCoCount = collaborativeCandidates.Values.Max();
        return maxCoCount > 0 ? (double)coCount / maxCoCount : 0.0;
    }

    /// <summary>
    /// Popularity: Normalized (views + 5×bookings) within the scoring window.
    /// Bookings are weighted 5× more than views as a stronger signal.
    /// Returns a score in [0, 1].
    /// </summary>
    private static double ScorePopularity(
        Guid packageId,
        Dictionary<Guid, PopularityData> popularity,
        int maxPopularity)
    {
        if (!popularity.TryGetValue(packageId, out var data))
            return 0.0;

        var raw = data.ViewCount + (data.BookingCount * 5);
        return maxPopularity > 0 ? (double)raw / maxPopularity : 0.0;
    }

    /// <summary>
    /// Recency: Exponential decay with a configurable half-life.
    /// Newer packages get a higher score. Returns a score in [0, 1].
    /// Formula: exp(-λ × ageDays) where λ = ln(2) / halfLife
    /// </summary>
    private static double ScoreRecency(DateTimeOffset createdAt)
    {
        var ageDays = (DateTimeOffset.UtcNow - createdAt).TotalDays;
        if (ageDays < 0) ageDays = 0;

        var lambda = Math.Log(2) / RecencyHalfLifeDays;
        return Math.Exp(-lambda * ageDays);
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  HELPER METHODS
    // ═════════════════════════════════════════════════════════════════════════

    private HashSet<TourType> GetUserTourTypePreferences(List<int> interestIds)
    {
        var tourTypes = new HashSet<TourType>();
        foreach (var interestId in interestIds)
        {
            if (InterestToTourTypeMap.TryGetValue(interestId, out var tourType))
            {
                // Decompose composite flags (e.g., Nature | Adventure)
                foreach (var t in DecomposeTourTypes(tourType))
                    tourTypes.Add(t);
            }
        }
        return tourTypes;
    }

    private static HashSet<TourType> DecomposeTourTypes(TourType flags)
    {
        var result = new HashSet<TourType>();
        foreach (TourType value in Enum.GetValues<TourType>())
        {
            if (value != TourType.None && flags.HasFlag(value))
                result.Add(value);
        }
        return result;
    }

    private static Dictionary<Guid, int> GetCollaborativeCandidates(
        HashSet<Guid> bookedIds,
        Dictionary<Guid, HashSet<Guid>> coOccurrence)
    {
        var candidates = new Dictionary<Guid, int>();

        foreach (var bookedId in bookedIds)
        {
            if (!coOccurrence.TryGetValue(bookedId, out var related))
                continue;

            foreach (var relatedId in related)
            {
                if (bookedIds.Contains(relatedId))
                    continue; // Skip already booked

                if (!candidates.ContainsKey(relatedId))
                    candidates[relatedId] = 0;
                candidates[relatedId]++;
            }
        }

        return candidates;
    }

    private async Task<Dictionary<Guid, HashSet<Guid>>> GetCachedCoOccurrenceAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var cached = await cache.GetAsync<Dictionary<Guid, HashSet<Guid>>>(
                CoOccurrenceCacheKey, cancellationToken);

            if (cached is not null)
                return cached;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read co-occurrence cache, computing fresh");
        }

        var coOccurrence = await repository.GetCoOccurrenceMapAsync(
            ScoringWindowDays, cancellationToken);

        try
        {
            await cache.SetAsync(CoOccurrenceCacheKey, coOccurrence,
                CoOccurrenceCacheTtl, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache co-occurrence map");
        }

        return coOccurrence;
    }

    private static string DetermineReason(
        double contentScore, double collaborativeScore, double popularityScore, string contentReason)
    {
        // Pick the dominant signal as the reason
        if (contentScore >= collaborativeScore && contentScore >= popularityScore && !string.IsNullOrEmpty(contentReason))
            return contentReason;

        if (collaborativeScore >= contentScore && collaborativeScore >= popularityScore && collaborativeScore > 0)
            return "👥 Travelers like you also loved this";

        if (popularityScore > 0)
            return "🔥 Popular choice this month";

        return "⭐ Recommended for you";
    }

    /// <summary>
    /// Takes scored packages and applies diversity injection:
    /// ensures at least MinDiversityCategories different TourType categories
    /// appear in the final results.
    /// </summary>
    private static List<RecommendedPackageResult> ApplyDiversityAndSelect(
        List<(PackageScoringData Package, double Score, string Reason)> scored,
        int count)
    {
        var sorted = scored.OrderByDescending(s => s.Score).ToList();
        var selected = new List<(PackageScoringData Package, double Score, string Reason)>();
        var categorySet = new HashSet<TourType>();

        // Phase 1: Greedily pick top-scored items, tracking category coverage
        foreach (var item in sorted)
        {
            if (selected.Count >= count)
                break;

            selected.Add(item);
            foreach (var t in DecomposeTourTypes(item.Package.TourTypes))
                categorySet.Add(t);
        }

        // Phase 2: If we lack diversity, swap lower-ranked same-category items
        // with highest-ranked items from underrepresented categories
        if (categorySet.Count < MinDiversityCategories && selected.Count >= MinDiversityCategories)
        {
            var remaining = sorted.Except(selected).ToList();
            foreach (var candidate in remaining)
            {
                var candidateTypes = DecomposeTourTypes(candidate.Package.TourTypes);
                var newTypes = candidateTypes.Except(categorySet).ToList();

                if (newTypes.Count > 0 && selected.Count > 0)
                {
                    // Find the lowest-scored item in selected that shares a category
                    // with an already-represented category
                    var swapTarget = selected
                        .OrderBy(s => s.Score)
                        .FirstOrDefault(s =>
                        {
                            var types = DecomposeTourTypes(s.Package.TourTypes);
                            return types.All(t => categorySet.Count(c => c == t) > 0) && types.Count > 0;
                        });

                    if (swapTarget.Package is not null)
                    {
                        selected.Remove(swapTarget);
                        selected.Add(candidate);
                        foreach (var t in newTypes)
                            categorySet.Add(t);
                    }

                    if (categorySet.Count >= MinDiversityCategories)
                        break;
                }
            }
        }

        // Re-sort after potential swaps
        return selected
            .OrderByDescending(s => s.Score)
            .Select(s => new RecommendedPackageResult(
                s.Package.Id,
                s.Package.Title,
                s.Package.AdultPrice,
                s.Package.DurationHours,
                s.Package.MainImageUrl,
                s.Package.TourTypes.ToString(),
                s.Package.Views,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();
    }

    /// <summary>
    /// Aggregates per-package scores to produce per-location scores.
    /// For each location, considers all associated packages' scores:
    /// - Personalized: 40% max + 30% avg + 20% rating + 10% density
    /// - Trending:     50% max + 30% rating + 20% density
    /// </summary>
    private static List<RecommendedLocationResult> AggregateLocationScores(
        List<LocationScoringData> locations,
        Dictionary<Guid, (double Score, string Reason)> packageScores,
        int count,
        bool isPersonalized)
    {
        // Compute max rating and max package count for normalization
        var maxRating = locations.Count > 0 ? (double)locations.Max(l => l.Rating) : 1.0;
        if (maxRating == 0) maxRating = 1.0;

        var maxPackageCount = locations.Count > 0 ? locations.Max(l => l.AssociatedPackageIds.Count) : 1;
        if (maxPackageCount == 0) maxPackageCount = 1;

        var scoredLocations = new List<(LocationScoringData Location, double Score, string Reason, int PkgCount)>();

        foreach (var loc in locations)
        {
            // Get scores for this location's packages
            var locPackageScores = loc.AssociatedPackageIds
                .Where(pid => packageScores.ContainsKey(pid))
                .Select(pid => packageScores[pid])
                .ToList();

            var pkgCount = locPackageScores.Count;

            double locationScore;
            string reason;

            if (pkgCount == 0)
            {
                // No active packages → still show the location but with a low score based on rating
                var normalizedRating = (double)loc.Rating / maxRating;
                locationScore = 0.10 * normalizedRating;
                reason = "📍 Discover this place";
            }
            else
            {
                var maxScore = locPackageScores.Max(s => s.Score);
                var avgScore = locPackageScores.Average(s => s.Score);
                var normalizedRating = (double)loc.Rating / maxRating;
                var normalizedDensity = (double)pkgCount / maxPackageCount;

                if (isPersonalized)
                {
                    // Personalized: 40% max + 30% avg + 20% rating + 10% density
                    locationScore = (0.40 * maxScore) + (0.30 * avgScore) + (0.20 * normalizedRating) + (0.10 * normalizedDensity);
                }
                else
                {
                    // Trending: 50% max + 30% rating + 20% density
                    locationScore = (0.50 * maxScore) + (0.30 * normalizedRating) + (0.20 * normalizedDensity);
                }

                // Pick the reason from the highest-scoring package
                reason = locPackageScores.OrderByDescending(s => s.Score).First().Reason;
            }

            scoredLocations.Add((loc, locationScore, reason, pkgCount));
        }

        return scoredLocations
            .OrderByDescending(s => s.Score)
            .Take(count)
            .Select(s => new RecommendedLocationResult(
                s.Location.Id,
                s.Location.Name,
                s.Location.MainImageUrl,
                s.Location.Rating,
                s.Location.Category.ToString(),
                s.PkgCount,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();
    }


    public async Task<List<RecommendedUnitResult>> GetPersonalizedUnitsAsync(
        Guid userId, int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating personalized unit recommendations for user {UserId}", userId);

        // 1. Load all data sequentially
        var candidates = await repository.GetCandidateUnitsAsync(cancellationToken);
        var userProfile = await repository.GetUserProfileDataAsync(userId, cancellationToken);
        var interactions = await repository.GetUserUnitInteractionsAsync(userId, ScoringWindowDays, cancellationToken);
        var bookedIds = (await repository.GetUserBookedUnitIdsAsync(userId, cancellationToken)).ToHashSet();
        var popularity = await repository.GetUnitPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        // 2. Load co-occurrence map (cached)
        var coOccurrence = await GetCachedUnitCoOccurrenceAsync(cancellationToken);

        // 3. Compute user-specific signals
        var favoritedIds = interactions
            .Where(i => i.Type == InteractionType.Favorite)
            .Select(i => i.EntityId)
            .ToHashSet();
        var viewedIds = interactions
            .Where(i => i.Type == InteractionType.View)
            .Select(i => i.EntityId)
            .ToHashSet();

        // 4. Find collaborative candidates
        var collaborativeCandidates = GetCollaborativeCandidates(bookedIds, coOccurrence);

        // 5. Compute normalization denominators
        var maxViews = candidates.Count > 0 ? candidates.Max(c => c.Views) : 1;
        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        // 6. Score each candidate
        var scoredUnits = new List<(HousingUnitScoringData Unit, double Score, string Reason)>();

        foreach (var unit in candidates)
        {
            // Skip already-booked units
            if (bookedIds.Contains(unit.Id))
                continue;

            var (contentScore, contentReason) = ScoreUnitContentBased(unit, userProfile);
            var collaborativeScore = ScoreCollaborative(unit.Id, collaborativeCandidates);
            var popularityScore = ScorePopularity(unit.Id, popularity, maxPopularity);
            var recencyScore = ScoreRecency(unit.CreatedAt);

            // Boost for favorited but not yet booked
            double favoriteBoost = favoritedIds.Contains(unit.Id) ? 0.10 : 0.0;
            // Small boost for viewed
            double viewBoost = viewedIds.Contains(unit.Id) ? 0.03 : 0.0;

            var finalScore =
                (W_Content * contentScore) +
                (W_Collaborative * collaborativeScore) +
                (W_Popularity * popularityScore) +
                (W_Recency * recencyScore) +
                favoriteBoost + viewBoost;

            var reason = DetermineReason(contentScore, collaborativeScore, popularityScore, contentReason);

            scoredUnits.Add((unit, finalScore, reason));
        }

        // 7. Sort and select top N
        var results = scoredUnits
            .OrderByDescending(s => s.Score)
            .Take(count)
            .Select(s => new RecommendedUnitResult(
                s.Unit.Id,
                s.Unit.Title,
                s.Unit.PricePerNight,
                s.Unit.MainImageUrl,
                s.Unit.AddressDetails,
                s.Unit.Rating,
                s.Unit.Views,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();

        logger.LogInformation("Generated {Count} personalized unit recommendations for user {UserId}", results.Count, userId);

        return results;
    }

    public async Task<List<RecommendedUnitResult>> GetTrendingUnitsAsync(
        int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating trending unit recommendations (anonymous/cold-start)");

        var candidates = await repository.GetCandidateUnitsAsync(cancellationToken);
        var popularity = await repository.GetUnitPopularityStatsAsync(ScoringWindowDays, cancellationToken);

        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        var scored = candidates.Select(unit =>
        {
            var popScore = ScorePopularity(unit.Id, popularity, maxPopularity);
            var recScore = ScoreRecency(unit.CreatedAt);
            var finalScore = (0.65 * popScore) + (0.35 * recScore);

            return (Unit: unit, Score: finalScore, Reason: "🔥 Popular choice this month");
        }).ToList();

        var results = scored
            .OrderByDescending(s => s.Score)
            .Take(count)
            .Select(s => new RecommendedUnitResult(
                s.Unit.Id,
                s.Unit.Title,
                s.Unit.PricePerNight,
                s.Unit.MainImageUrl,
                s.Unit.AddressDetails,
                s.Unit.Rating,
                s.Unit.Views,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();

        logger.LogInformation("Generated {Count} trending unit recommendations", results.Count);

        return results;
    }

    private static (double Score, string Reason) ScoreUnitContentBased(
        HousingUnitScoringData unit,
        UserProfileData? profile)
    {
        if (profile is null)
            return (0.0, string.Empty);

        double budgetScore = 0.0;
        if (profile.BudgetTier is not null && BudgetPriceRanges.TryGetValue(profile.BudgetTier.Value, out var range))
        {
            if (unit.PricePerNight >= range.Min && unit.PricePerNight <= range.Max)
                budgetScore = 1.0;
            else
            {
                var distance = unit.PricePerNight < range.Min
                    ? (double)(range.Min - unit.PricePerNight) / (double)range.Min
                    : (double)(unit.PricePerNight - range.Max) / (double)unit.PricePerNight;
                budgetScore = Math.Max(0.0, 1.0 - distance);
            }
        }

        double styleScore = 0.0;
        string matchedStyleReason = string.Empty;

        if (profile.TravelStyle is not null)
        {
            switch (profile.TravelStyle.Value)
            {
                case TravelStyle.Single:
                    if (unit.MaxGuests <= 2 && unit.BedRooms <= 1)
                    {
                        styleScore = 1.0;
                        matchedStyleReason = "👤 Ideal space for solo travelers";
                    }
                    else if (unit.MaxGuests <= 3)
                    {
                        styleScore = 0.5;
                    }
                    break;

                case TravelStyle.Couple:
                    if (unit.MaxGuests >= 2 && unit.MaxGuests <= 3 && unit.BedRooms == 1)
                    {
                        styleScore = 1.0;
                        matchedStyleReason = "💑 Perfect cozy stay for couples";
                    }
                    else if (unit.MaxGuests >= 2 && unit.MaxGuests <= 4)
                    {
                        styleScore = 0.6;
                    }
                    break;

                case TravelStyle.Friends:
                    if (unit.MaxGuests >= 2 && unit.NumberOfBeds >= 2)
                    {
                        styleScore = 1.0;
                        matchedStyleReason = "👥 Great setup for groups of friends";
                    }
                    else if (unit.MaxGuests >= 2)
                    {
                        styleScore = 0.5;
                    }
                    break;

                case TravelStyle.Family:
                    if (unit.MaxGuests >= 3 && unit.BedRooms >= 2)
                    {
                        styleScore = 1.0;
                        matchedStyleReason = "👨‍👩‍👧‍👦 Family-friendly spacious stay";
                    }
                    else if (unit.MaxGuests >= 3)
                    {
                        styleScore = 0.5;
                    }
                    break;
            }
        }

        // Combine: 50% budget match, 50% travel style match
        var finalScore = (0.5 * budgetScore) + (0.5 * styleScore);

        var reason = !string.IsNullOrEmpty(matchedStyleReason) && styleScore >= 0.8
            ? matchedStyleReason
            : budgetScore >= 0.8
                ? "💰 Fits your budget preferences"
                : string.Empty;

        return (finalScore, reason);
    }

    private async Task<Dictionary<Guid, HashSet<Guid>>> GetCachedUnitCoOccurrenceAsync(
        CancellationToken cancellationToken)
    {
        const string UnitCoOccurrenceCacheKey = "recommendation:unit-co-occurrence";
        try
        {
            var cached = await cache.GetAsync<Dictionary<Guid, HashSet<Guid>>>(
                UnitCoOccurrenceCacheKey, cancellationToken);

            if (cached is not null)
                return cached;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read unit co-occurrence cache, computing fresh");
        }

        var coOccurrence = await repository.GetUnitCoOccurrenceMapAsync(
            ScoringWindowDays, cancellationToken);

        try
        {
            await cache.SetAsync(UnitCoOccurrenceCacheKey, coOccurrence,
                CoOccurrenceCacheTtl, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache unit co-occurrence map");
        }

        return coOccurrence;
    }

    public async Task<List<RecommendedGuideResult>> GetPersonalizedGuidesAsync(
        Guid userId, int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating personalized tour guide recommendations for user {UserId}", userId);

        // 1. Load all data sequentially
        var candidates = await repository.GetCandidateGuidesAsync(cancellationToken);
        var userInterests = await repository.GetUserInterestIdsAsync(userId, cancellationToken);
        var userProfile = await repository.GetUserProfileDataAsync(userId, cancellationToken);
        var interactions = await repository.GetUserGuideInteractionsAsync(userId, ScoringWindowDays, cancellationToken);
        var bookedIds = (await repository.GetUserBookedGuideIdsAsync(userId, cancellationToken)).ToHashSet();
        var popularity = await repository.GetGuidePopularityStatsAsync(ScoringWindowDays, cancellationToken);

        // 2. Load co-occurrence map (cached)
        var coOccurrence = await GetCachedGuideCoOccurrenceAsync(cancellationToken);

        // 3. Compute user-specific signals
        var userTourTypes = GetUserTourTypePreferences(userInterests);
        var favoritedIds = interactions
            .Where(i => i.Type == InteractionType.Favorite)
            .Select(i => i.EntityId)
            .ToHashSet();
        var viewedIds = interactions
            .Where(i => i.Type == InteractionType.View)
            .Select(i => i.EntityId)
            .ToHashSet();

        // 4. Find collaborative candidates
        var collaborativeCandidates = GetCollaborativeCandidates(bookedIds, coOccurrence);

        // 5. Compute normalization denominators
        var maxViews = candidates.Count > 0 ? candidates.Max(c => c.Views) : 1;
        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        // 6. Score each candidate
        var scoredGuides = new List<(GuideScoringData Guide, double Score, string Reason)>();

        foreach (var guide in candidates)
        {
            // Skip already-booked guides
            if (bookedIds.Contains(guide.UserId))
                continue;

            var (contentScore, contentReason) = ScoreGuideContentBased(guide, userTourTypes, userProfile);
            var collaborativeScore = ScoreCollaborative(guide.UserId, collaborativeCandidates);
            var popularityScore = ScorePopularity(guide.UserId, popularity, maxPopularity);
            var recencyScore = ScoreRecency(guide.CreatedAt);

            // Boost for favorited but not yet booked
            double favoriteBoost = favoritedIds.Contains(guide.UserId) ? 0.10 : 0.0;
            // Small boost for viewed
            double viewBoost = viewedIds.Contains(guide.UserId) ? 0.03 : 0.0;

            var finalScore =
                (W_Content * contentScore) +
                (W_Collaborative * collaborativeScore) +
                (W_Popularity * popularityScore) +
                (W_Recency * recencyScore) +
                favoriteBoost + viewBoost;

            var reason = DetermineReason(contentScore, collaborativeScore, popularityScore, contentReason);

            scoredGuides.Add((guide, finalScore, reason));
        }

        // 7. Sort and select top N
        var results = scoredGuides
            .OrderByDescending(s => s.Score)
            .Take(count)
            .Select(s => new RecommendedGuideResult(
                s.Guide.UserId,
                s.Guide.FullName,
                s.Guide.ProfileImageUrl,
                s.Guide.BaseRate,
                s.Guide.YearsOfExperience,
                s.Guide.IsSuperGuide,
                s.Guide.AverageRating,
                s.Guide.ReviewCount,
                s.Guide.Views,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();

        logger.LogInformation("Generated {Count} personalized tour guide recommendations for user {UserId}", results.Count, userId);

        return results;
    }

    public async Task<List<RecommendedGuideResult>> GetTrendingGuidesAsync(
        int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating trending tour guide recommendations (anonymous/cold-start)");

        var candidates = await repository.GetCandidateGuidesAsync(cancellationToken);
        var popularity = await repository.GetGuidePopularityStatsAsync(ScoringWindowDays, cancellationToken);

        var maxPopularity = popularity.Count > 0
            ? popularity.Values.Max(p => p.ViewCount + p.BookingCount * 5)
            : 1;

        var scored = candidates.Select(guide =>
        {
            var popScore = ScorePopularity(guide.UserId, popularity, maxPopularity);
            var recScore = ScoreRecency(guide.CreatedAt);
            var finalScore = (0.65 * popScore) + (0.35 * recScore);

            return (Guide: guide, Score: finalScore, Reason: "🔥 Highly requested guide");
        }).ToList();

        var results = scored
            .OrderByDescending(s => s.Score)
            .Take(count)
            .Select(s => new RecommendedGuideResult(
                s.Guide.UserId,
                s.Guide.FullName,
                s.Guide.ProfileImageUrl,
                s.Guide.BaseRate,
                s.Guide.YearsOfExperience,
                s.Guide.IsSuperGuide,
                s.Guide.AverageRating,
                s.Guide.ReviewCount,
                s.Guide.Views,
                Math.Round(s.Score, 4),
                s.Reason))
            .ToList();

        logger.LogInformation("Generated {Count} trending tour guide recommendations", results.Count);

        return results;
    }

    private static (double Score, string Reason) ScoreGuideContentBased(
        GuideScoringData guide,
        HashSet<TourType> userTourTypes,
        UserProfileData? profile)
    {
        // 1. TourType Preferences Match
        double tourTypeScore = 0.0;
        string matchedType = string.Empty;

        if (userTourTypes.Count > 0 && guide.TourTypes != TourType.None)
        {
            var guideTypes = DecomposeTourTypes(guide.TourTypes);
            var intersection = userTourTypes.Intersect(guideTypes).Count();
            var union = userTourTypes.Union(guideTypes).Count();

            tourTypeScore = union > 0 ? (double)intersection / union : 0.0;

            if (intersection > 0)
            {
                matchedType = userTourTypes.Intersect(guideTypes).First().ToString();
            }
        }

        // 2. Budget matching for guide base rate
        double budgetScore = 0.0;
        if (profile?.BudgetTier is not null && guide.BaseRate.HasValue)
        {
            var rate = guide.BaseRate.Value;
            switch (profile.BudgetTier.Value)
            {
                case BudgetTier.Low:
                    if (rate <= 300m)
                        budgetScore = 1.0;
                    else
                        budgetScore = Math.Max(0.0, (double)(1.0m - (rate - 300m) / rate));
                    break;

                case BudgetTier.Medium:
                    if (rate >= 200m && rate <= 800m)
                        budgetScore = 1.0;
                    else
                    {
                        var distance = rate < 200m
                            ? (double)(200m - rate) / 200.0
                            : (double)(rate - 800m) / (double)rate;
                        budgetScore = Math.Max(0.0, 1.0 - distance);
                    }
                    break;

                case BudgetTier.Luxury:
                    if (rate >= 600m)
                        budgetScore = 1.0;
                    else
                        budgetScore = Math.Max(0.0, (double)(rate / 600m));
                    break;
            }
        }

        // 3. Experience and SuperGuide status
        double superGuideScore = guide.IsSuperGuide ? 1.0 : 0.0;
        double experienceScore = guide.YearsOfExperience.HasValue
            ? Math.Min(guide.YearsOfExperience.Value / 10.0, 1.0)
            : 0.0;

        // Combined content score
        var finalScore = (0.50 * tourTypeScore) + (0.30 * budgetScore) + (0.10 * superGuideScore) + (0.10 * experienceScore);

        var reason = !string.IsNullOrEmpty(matchedType)
            ? $"✨ Offers {matchedType} tours you prefer"
            : guide.IsSuperGuide
                ? "👑 Highly-rated SuperGuide"
                : budgetScore > 0.8
                    ? "💰 Fits your budget tier"
                    : string.Empty;

        return (finalScore, reason);
    }

    private async Task<Dictionary<Guid, HashSet<Guid>>> GetCachedGuideCoOccurrenceAsync(
        CancellationToken cancellationToken)
    {
        const string GuideCoOccurrenceCacheKey = "recommendation:guide-co-occurrence";
        try
        {
            var cached = await cache.GetAsync<Dictionary<Guid, HashSet<Guid>>>(
                GuideCoOccurrenceCacheKey, cancellationToken);

            if (cached is not null)
                return cached;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read guide co-occurrence cache, computing fresh");
        }

        var coOccurrence = await repository.GetGuideCoOccurrenceMapAsync(
            ScoringWindowDays, cancellationToken);

        try
        {
            await cache.SetAsync(GuideCoOccurrenceCacheKey, coOccurrence,
                CoOccurrenceCacheTtl, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache guide co-occurrence map");
        }

        return coOccurrence;
    }
}
