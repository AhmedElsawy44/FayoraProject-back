namespace Fayora.Contracts.TourGuideModule.UpdatePackageOccurrence;

public record UpdatePackageOccurrenceRequest(
    DateOnly NewDate,
    int NewAvailableSeats
);
