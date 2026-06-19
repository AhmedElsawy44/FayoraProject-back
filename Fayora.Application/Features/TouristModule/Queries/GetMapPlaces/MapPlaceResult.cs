namespace Fayora.Application.Features.TouristModule.Queries.GetMapPlaces
{
    public record MapPlaceResult(
        string Id,
        string Name,
        decimal Latitude,
        decimal Longitude,
        string ImageUrl,
        string Description,
        string Type);
}
