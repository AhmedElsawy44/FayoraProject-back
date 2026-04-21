using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.GetAllPackages
{

    public record GetAllPackagesResponse(
        List<PackageItemResponse> Items,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages);

    public record PackageItemResponse(
        Guid PackageId,
        Guid CompanyId,
        string CompanyName,
        string Title,
        string Description,
        int TourTypes,
        int DurationHours,
        int MaxCapacity,
        int AvailableSpots,
        decimal AdultPrice,
        decimal ChildPrice,
        string? MainImageUrl,
        DateOnly StartDate,
        DateOnly EndDate);
}
