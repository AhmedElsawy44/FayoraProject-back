namespace Fayora.Domain.Entities.Shared
{
    public class City
    {
        public int CityId { get; init; }
        public string Name { get; init; }
        public string CountryCode { get; init; }

        public City(int cityId, string name, string countryCode)
        {
            CityId = cityId;
            Name = name;
            CountryCode = countryCode;
        }

        private City()
        {
            Name = string.Empty;
            CountryCode = string.Empty;
        }
    }
}
