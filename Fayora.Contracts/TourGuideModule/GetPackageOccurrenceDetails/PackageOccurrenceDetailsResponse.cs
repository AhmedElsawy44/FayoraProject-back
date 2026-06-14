namespace Fayora.Contracts.TourGuideModule.GetPackageOccurrenceDetails;

public record PackageOccurrenceDetailsResponse(
    string PackageName,
    string MainImageUrl,
    DateOnly OccurrenceDate,
    int TotalBookedSeats,
    int AvailableSeats,
    int MaxCapacity,
    decimal TotalRevenue,
    List<OccurrenceAttendeeResponse> Attendees
);

public record OccurrenceAttendeeResponse(
    Guid UserId,
    string FullName,
    string? ProfileImageUrl,
    int SeatsCount,
    string PaymentStatus,
    string? MeetingPointName,
    TimeOnly? MeetingPointTime
);
