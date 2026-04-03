using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Common;

public static class TourGuideErrors
{
    public static readonly Error HasRole = Error.Validation(
        "TourGuide.HasRole",
        "User already has a role assigned, cannot be a tour guide."
    );

    public static readonly Error InvalidPricingUnit = Error.Validation(
        "TourGuide.InvalidPricingUnit",
        "The specified pricing unit is invalid."
    );

    public static readonly Error GuidIdNotExist = Error.Validation(
        "TourGuide.GuidIdNotExist",
        "Tour guide ID does not exist in the current context."
    );

    public static readonly Error InvalidTourType = Error.Validation(
        "TourGuide.InvalidTourType",
        "The specified tour type is invalid."
    );

    public static readonly Error InvalidTransportType = Error.Validation(
        "TourGuide.InvalidTransportType",
        "The specified transport type is invalid."
    );

    public static readonly Error GuideNotFound = Error.NotFound(
        "TourGuide.GuideNotFound",
        "The specified tour guide was not found."
    );
}
