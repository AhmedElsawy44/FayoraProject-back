using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class PackageActivity : BaseEntity<Guid>
{
    public Guid PackageId { get; private set; }
    public GeoPoint Place { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset ActivityTime { get; private set; }
    public bool IsOptional { get; private set; }

    public static Result<PackageActivity> Create(Guid packageId, decimal latitude, decimal longitude, string? description, DateTimeOffset activityTime, bool isOptional)
    {
        var place = GeoPoint.Create(latitude, longitude);
        if (place.IsError) return place.Errors;

        return new PackageActivity
        {
            Id = Guid.NewGuid(),
            PackageId = packageId,
            Place = place.Value,
            Description = description,
            ActivityTime = activityTime,
            IsOptional = isOptional
        };
    }

    private PackageActivity() { }
}
