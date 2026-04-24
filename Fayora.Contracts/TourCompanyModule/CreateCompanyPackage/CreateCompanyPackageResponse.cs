namespace Fayora.Contracts.TourCompanyModule.CreateCompanyPackage
{
    public record CreateCompanyPackageResponse(
        Guid PackageId,
        Guid CompanyId,
        string Title,
        string Status,
        DateTimeOffset CreatedAt);
}
