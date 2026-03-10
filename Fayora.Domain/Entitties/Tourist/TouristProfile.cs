using Fayora.Domain.Enums.TouristModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entitties.Tourist;

public class TouristProfile : BaseEntity<Guid>
{
    public Guid UserId { get; init; }
    public TravelStyle? TravelStyle { get; private set; }
    public BudgetTier? BudgetTier { get; private set; }
    public TravelCompanionType? TravelCompanionType { get; private set; }
    public GeoPoint LastLocation { get; private set; } = new GeoPoint(0, 0);
    public DateTimeOffset? LastLocationUpdate { get; private set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool OnboardingComplete { get; private set; }

    private readonly List<TouristInterest> _interests = [];
    public IReadOnlyCollection<TouristInterest> Interests => _interests.AsReadOnly();

    private readonly List<Wishlist> _wishlists = [];
    public IReadOnlyCollection<Wishlist> Wishlists => _wishlists.AsReadOnly();

    public TouristProfile(Guid userId, TravelStyle? travelStyle, BudgetTier? budgetTier, TravelCompanionType? travelCompanionType)
    {
        UserId = userId;
        TravelStyle = travelStyle;
        BudgetTier = budgetTier;
        TravelCompanionType = travelCompanionType;

        OnboardingComplete = budgetTier is not null && travelCompanionType is not null && travelStyle is not null;
    }

    public void UpdateLocation(GeoPoint newLocation)
    {
        LastLocation = newLocation;
        LastLocationUpdate = DateTimeOffset.UtcNow;
    }
    public void AddInterest(int interestId)
    {
        if (!_interests.Any(i => i.InterestId == interestId))
            _interests.Add(new TouristInterest(this.Id, interestId));
    }
    public void RemoveInterest(int interestId)
    {
        var interest = _interests.FirstOrDefault(i => i.InterestId == interestId);
        if (interest is not null)
        {
            _interests.Remove(interest);
        }
    }

    public void AddToWishlist(string itemType, Guid itemId)
    {
        if (!_wishlists.Any(w => w.ItemType == itemType && w.ItemId == itemId))
            _wishlists.Add(new Wishlist(this.Id, itemType, itemId));
    }


    public void RemoveFromWishlist(string itemType, Guid itemId)
    {
        var wishlistItem = _wishlists.FirstOrDefault(w => w.ItemType == itemType && w.ItemId == itemId);
        if (wishlistItem is not null)
        {
            _wishlists.Remove(wishlistItem);
        }
    }
    private TouristProfile() { }
}