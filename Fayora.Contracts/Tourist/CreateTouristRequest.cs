namespace Fayora.Contracts.Tourist;

public record CreateTouristRequest(
    string? BudgetTier,
    string? TravelStyle,
    HashSet<int> InterestIds);
