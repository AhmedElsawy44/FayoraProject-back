namespace Fayora.Contracts.TourGuideModule.CreatePackageOccurrences;

public record CreatePackageOccurrencesRequest(
    List<OccurrenceItemRequest> Occurrences
);

public record OccurrenceItemRequest(
    DateOnly Date,
    int AvailableSeats
);
