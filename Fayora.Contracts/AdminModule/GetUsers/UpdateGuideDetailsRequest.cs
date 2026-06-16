namespace Fayora.Contracts.AdminModule.GetUsers;

public record UpdateGuideDetailsRequest(
    decimal? BaseRate,
    string? PricingUnit,
    int? YearsOfExperience,
    string? LicenseNumber,
    DateOnly? LicenseExpiryDate,
    bool IsSuperGuide,
    decimal AverageRating,
    int ReviewCount,
    int CompletedToursCount,
    bool IsAvailableForBooking,
    decimal ResponseRate,
    decimal CancellationRate,
    string Status,
    string? AdminNotes
);
