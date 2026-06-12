namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageOccurrenceDetails;

public record GetPackageOccurrenceDetailsResult(
    string PackageName,
    string MainImageUrl,
    DateOnly OccurrenceDate,
    int TotalBookedSeats,
    int AvailableSeats,
    int MaxCapacity,
    decimal TotalRevenue,
    List<OccurrenceAttendeeResult> Attendees
);

public record OccurrenceAttendeeResult(
    Guid UserId,
    string FullName,
    string? ProfileImageUrl,
    int SeatsCount,
    string PaymentStatus
);
