using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.UpdateCompanyPackage
{
    public record UpdateCompanyPackageRequest(
        string Title,
        string Description,
        TourTypeDto TourTypes,
        int DurationHours,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal DepartureLat,
        decimal DepartureLng,
        int MaxCapacity,
        decimal AdultPrice,
        decimal ChildPrice,
        string? CancellationPolicy,
        string? MainImageUrl,
        string? MainVideoUrl,
        string? GuestRequirements,
        List<string> IncludedItems,
        List<string> ExcludedItems);
}
