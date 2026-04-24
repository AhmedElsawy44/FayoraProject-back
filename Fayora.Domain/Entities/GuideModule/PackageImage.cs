using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class PackageImage
{
    public Guid Id { get; private set; }
    public Guid PackageId { get; private set; }
    public FileUrl ImageUrl { get; private set; } = null!;

    public PackageImage(Guid packageId, FileUrl imageUrl)
    {
        Id = Guid.CreateVersion7();
        PackageId = packageId;
        ImageUrl = imageUrl;
    }

    protected PackageImage() { }
}
