namespace Fayora.Contracts.AccommodationModule.Responses
{
    public record GetAllUnitsByTypeResponse(
    string Title,
    decimal PricePerNight,
    decimal DiscountedPricePerNight,
    decimal Rating,
    string MainImageUrl,
    string AddressDetails
    );
}
