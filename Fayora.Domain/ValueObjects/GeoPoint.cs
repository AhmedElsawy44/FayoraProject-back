using Fayora.Domain.Common.ValueObjects;

namespace Fayora.Domain.ValueObjects;

public class GeoPoint : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public GeoPoint(double latitude, double longitude)
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
