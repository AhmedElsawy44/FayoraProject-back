namespace Fayora.Application.Features.TourCompanyModule.Queries.GetCompanyPackages
{
    public record GetCompanyPackagesResult(
        Guid PackageId,
        Guid CompanyId,
        string Title,
        string Description,
        int TourTypes,
        int DurationHours,
        int MaxCapacity,
        int AvailableSpots,
        decimal AdultPrice,
        decimal ChildPrice,
        bool IsActive,
        int Views,
        string? MainImageUrl,
        DateOnly StartDate,
        DateOnly EndDate,
        DateTimeOffset CreatedAt);
}