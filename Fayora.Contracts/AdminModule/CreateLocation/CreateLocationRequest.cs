namespace Fayora.Contracts.AdminModule.CreateLocation
{
    public record CreateLocationRequest(
        string Name,
        string? Description,
        decimal Rating,
        decimal Latitude,
        decimal Longitude,
        LocationCategoryDto Category,
        string MainImageUrl,
       List<string> ImageUrls);



    public enum LocationCategoryDto
    {
        UnescoWorldHeritage,
        NaturalWonder,
        CulturalHub,
        Adventure,
        Beach,
        Historical
    }
}
