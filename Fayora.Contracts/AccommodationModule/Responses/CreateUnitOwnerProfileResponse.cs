namespace Fayora.Contracts.AccommodationModule.Responses;

public record CreateUnitOwnerProfileResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    int ExpiresIn);
