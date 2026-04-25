using Fayora.Domain.Common.Results;
using Fayora.Domain.Common.ValueObject;

namespace Fayora.Domain.ValueObjects;

public class GeoPoint : ValueObject
{
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }

    public static Result<GeoPoint> Create(decimal? latitude, decimal? longitude)
    {
        if (latitude < 0 || longitude < 0)
        {
            return Error.Validation("Latitude and Longitude must be non-negative.");
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
