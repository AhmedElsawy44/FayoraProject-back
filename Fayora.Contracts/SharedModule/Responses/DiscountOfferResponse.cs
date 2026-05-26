using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.SharedModule.Responses
{
    public record DiscountOfferResponse(
        Guid Id,
        Guid TargetId,
        string TargetType,        // "HousingUnit" | "GuidePackage" | "TourGuide"
        string Title,
        string? Description,
        string DiscountType,      // "Percentage" | "FixedAmount"
        decimal DiscountValue,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        string Status             // "Active" | "Expired" | "Cancelled"
    );
}
