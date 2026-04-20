using Fayora.Domain.Common.ValueObjects;

namespace Fayora.Domain.ValueObjects;

public class GeoPoint : ValueObject
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public GeoPoint(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
