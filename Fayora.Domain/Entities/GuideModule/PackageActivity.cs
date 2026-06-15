using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class PackageActivity : BaseEntity<Guid>
{
    public Guid PackageId { get; private set; }
    public GeoPoint? Place { get; private set; }
    public int? LocationId { get; private set; }
    public string Description { get; private set; } = default!;
    public TimeOnly ActivityTime { get; private set; }
    public bool IsOptional { get; private set; }


    public static Result<PackageActivity> Create(
        Guid packageId,
        string description,
        TimeOnly activityTime,
        bool isOptional,
        int? locationId = null,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        // التحقق من وجود أحد الخيارين على الأقل
        bool hasCoordinates = latitude.HasValue && longitude.HasValue;
        bool hasLocationId = locationId.HasValue;

        if (!hasCoordinates && !hasLocationId)
            return Error.Validation("Activity.LocationRequired",
                "Activity must have either a LocationId or coordinates (Latitude and Longitude).");

        GeoPoint? place = null;
        if (hasCoordinates)
        {
            var placeResult = GeoPoint.Create(latitude!.Value, longitude!.Value);
            if (placeResult.IsError) return placeResult.Errors;
            place = placeResult.Value;
        }

        return new PackageActivity
        {
            Id = Guid.NewGuid(),
            PackageId = packageId,
            Place = place,
            Description = description,
            ActivityTime = activityTime,
            IsOptional = isOptional,
            LocationId = locationId
        };
    }

    private PackageActivity() { }
}
