using Fayora.Domain.Enums.TouristModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TouristModule;

public class TouristProfile : BaseEntity<Guid>
{
    public Guid UserId { get; init; }
    public TravelStyle? TravelStyle { get; private set; }
    public BudgetTier? BudgetTier { get; private set; }
    public GeoPoint LastLocation { get; private set; } = null!;
    public DateTimeOffset? LastLocationUpdate { get; private set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool OnboardingComplete { get; private set; }

    private readonly List<int> _interests = [];
    public IReadOnlyCollection<int> Interests => _interests.AsReadOnly();

    private readonly List<Guid> _wishIds = [];
    public IReadOnlyCollection<Guid> WishIds => _wishIds.AsReadOnly();

    public TouristProfile(Guid userId, BudgetTier? budgetTier, TravelStyle? travelStyle)
    {
        UserId = userId;
        BudgetTier = budgetTier;
        TravelStyle = travelStyle;

        OnboardingComplete = budgetTier is not null && travelStyle is not null;
    }

    public void UpdateLocation(GeoPoint newLocation)
    {
        LastLocation = newLocation;
        LastLocationUpdate = DateTimeOffset.UtcNow;
    }

    public void AddInterests(IEnumerable<int>? interestIds)
    {
        if (interestIds is null || _interests.Any(_interests => interestIds.Contains(_interests)))
            return;
        _interests.AddRange(interestIds);
    }

    public void RemoveInterest(int interestId)
    {
        if (_interests.Contains(interestId))
        {
            _interests.Remove(interestId);
        }
    }

    public void AddToWishlist(Guid itemId)
    {
        if (!_wishIds.Any(w => w == itemId))
            return;
        _wishIds.Add(itemId);
    }


    public void RemoveFromWishlist(Guid itemId)
    {
        _wishIds.Remove(itemId);
    }

    private TouristProfile() { }
}