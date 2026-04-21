namespace Fayora.Contracts.AccommodationModule.Requests;

public record CreateUnitOwnerProfileRequest(
    string OwnerType,
    string NationalIdUrl,
    string? CommercialName = null,
    string? TaxRegistrationNumber = null);