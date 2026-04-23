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

    public static readonly Error PackageNotFound = Error.NotFound(
        "TourGuide.PackageNotFound",
        "The specified tour guide package was not found."
    );

    public static readonly Error UnauthorizedPackageModification = Error.Unauthorized(
        "TourGuide.UnauthorizedPackageModification",
        "The specified tour guide package was not authorized to be modified by that tour guide."
    );

    public static Error PackageIsAlreadyDeactivated = Error.Validation(
        "TourGuide.PackageIsAlreadyDeactivated",
        "The specified tour guide package was already deactivated."
    );

    public static Error PackageIsAlreadyActivated = Error.Validation(
        "TourGuide.PackageIsAlreadyActivated",
        "The specified tour guide package was already activated."
    );

    public static readonly Error TourGuideIsAlreadyExist = Error.Validation(
        "TourGuide.TourGuideIsAlreadyExist",
        "The user already has a tour guide profile."
    );

    public static readonly Error Unauthorized = Error.Unauthorized(
            "TourGuide.Unauthorized",
            "User is not authorized to perform this action."
    );
}
