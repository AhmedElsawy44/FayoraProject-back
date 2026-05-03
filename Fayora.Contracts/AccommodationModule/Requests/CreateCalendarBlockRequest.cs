namespace Fayora.Contracts.AccommodationModule.Requests;

public record CreateCalendarBlockRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    string BlockReason);
