namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage
{
    public record CreateCompanyPackageResult(
        Guid PackageId,
        Guid CompanyId,
        string Title,
        string Status,
        DateTimeOffset CreatedAt);
}