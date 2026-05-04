using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Errors;

public static class GuideErrors
{
    public static readonly Error PackageNotFound = Error.NotFound("TourGuide.PackageNotFound", "The requested package was not found.");

    public static readonly Error PackageOccurrenceOverlap = Error.Conflict("TourGuide.OccurrenceOverlap", "One or more requested dates already have an occurrence.");

    public static readonly Error DuplicateDatesInRequest = Error.Validation("TourGuide.DuplicateDates", "The request contains duplicate dates.");

    public static readonly Error OccurrenceNotFound = Error.NotFound("TourGuide.OccurrenceNotFound", "The requested occurrence was not found.");
}
