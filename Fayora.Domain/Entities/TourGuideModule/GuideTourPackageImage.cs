using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Domain.Entities.TourGuide;


public class GuideTourPackageImage : TourPackageImageBase
{
    public GuideTourPackageImage(Guid packageId, string imageUrl)
        : base(packageId, imageUrl) { }
    private GuideTourPackageImage() { }
}
