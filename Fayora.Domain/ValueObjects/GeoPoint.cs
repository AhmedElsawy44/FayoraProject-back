using Fayora.Domain.Common.Results;
using Fayora.Domain.Common.ValueObject;

namespace Fayora.Domain.ValueObjects;

public class GeoPoint : ValueObject
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public static Result<GeoPoint> Create(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return Error.Validation("Latitude must be between -90 and 90.");
        }
        if (longitude < -180 || longitude > 180)
        {
            return Error.Validation("Longitude must be between -180 and 180.");
        }
        return new GeoPoint
        {
            Latitude = latitude,
            Longitude = longitude
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
