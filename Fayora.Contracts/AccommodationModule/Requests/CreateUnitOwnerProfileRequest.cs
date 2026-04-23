namespace Fayora.Contracts.AccommodationModule.Requests;

public record CreateUnitOwnerProfileRequest(
    string OwnerType,
    string CommercialName);