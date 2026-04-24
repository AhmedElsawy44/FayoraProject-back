using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.SharedModule;

public class City
{
    public int CityId { get; init; }
    public string Name { get; init; }
    public string CountryCode { get; init; }
    public GeoPoint CenterCoordinates { get; init; }

    public City(int cityId, string name, string countryCode, GeoPoint centerCoordinates)
    {
        CityId = cityId;
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
