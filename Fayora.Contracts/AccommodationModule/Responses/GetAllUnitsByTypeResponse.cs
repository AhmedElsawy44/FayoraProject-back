namespace Fayora.Contracts.AccommodationModule.Responses
{
    public record GetAllUnitsByTypeResponse(
    string Title,
    decimal PricePerNight,
    decimal Rating,
    string MainImageUrl,
    string AddressDetails
    );
}
