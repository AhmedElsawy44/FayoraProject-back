using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TourCompanyModule;

public class CompanyTourPackageImage
{
    public Guid Id { get; private set; }
    public Guid PackageId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;

    private CompanyTourPackageImage()
    {
    }
}
