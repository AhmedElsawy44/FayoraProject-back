namespace Fayora.Contracts.TourGuideModule.UpdateTourGuide;

public record UpdateTourGuideRequest
(
    int YearsOfExperience,
    string PricingUnit,
    decimal BaseRate,
    List<int> CoveredCities
);
