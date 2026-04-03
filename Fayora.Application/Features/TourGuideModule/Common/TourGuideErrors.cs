using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Common;

public static class TourGuideErrors
{
    public static readonly Error HasRole = Error.Validation("TourGuide.HasRole", "User already has a role assigned, cannot be a tour guide."
    );

    public static readonly Error InvalidPricingUnit = Error.Validation("TourGuide.InvalidPricingUnit", "The specified pricing unit is invalid."
    );
}
