using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.SharedModule.Requests
{
    public record CreateDiscountOfferRequest(
        Guid TargetId,
        string TargetType,      // "HousingUnit" | "GuidePackage" | "TourGuide"
        string Title,
        string? Description,
        string DiscountType,    // "Percentage" | "FixedAmount"
        decimal DiscountValue,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        int? UsageLimit = null
    );

}
