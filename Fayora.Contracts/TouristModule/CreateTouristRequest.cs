namespace Fayora.Contracts.TouristModule;

public record CreateTouristRequest(
    string? BudgetTier,
    string? TravelStyle,
    HashSet<int> InterestIds);
