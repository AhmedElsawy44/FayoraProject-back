using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Entities.TourGuideModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TourCompanyModule;

public class CompanyTourPackage
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public TourCompany Company { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TourType TourTypes { get; private set; }
    public decimal AdultPrice { get; private set; }
    public decimal ChildPrice { get; private set; }
    public string? MainImageUrl { get; private set; }
    public string? MainVideoUrl { get; private set; }
    public string? CancellationPolicy { get; private set; }
    public GeoPoint DepartureLocation { get; private set; } = null!;

    private readonly List<string> _includedItems = [];
    public IReadOnlyCollection<string> IncludedItems => _includedItems.AsReadOnly();

    private readonly List<string> _excludedItems = [];
    public IReadOnlyCollection<string> ExcludedItems => _excludedItems.AsReadOnly();

    private readonly List<CompanyTourPackageImage> _images = [];
    public IReadOnlyCollection<CompanyTourPackageImage> Images => _images.AsReadOnly();

    private readonly List<PackageActivity> _activities = [];
    public IReadOnlyCollection<PackageActivity> Activities => _activities.AsReadOnly();

    private CompanyTourPackage()
    {
    }
}
