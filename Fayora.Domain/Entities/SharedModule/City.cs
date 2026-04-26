using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.SharedModule;

public class City : BaseEntity<int>
{
    public string Name { get; init; }
    public string CountryCode { get; init; }
    public GeoPoint CenterCoordinates { get; init; }

    public City(string name, string countryCode, GeoPoint centerCoordinates)
    {
        Name = name;
        CountryCode = countryCode;
        CenterCoordinates = centerCoordinates;
    }

    private City()
    {
        Name = string.Empty;
        CountryCode = string.Empty;
        CenterCoordinates = null!;
    }
}
