namespace Fayora.Contracts.AccommodationModule.Responses;

public record CreateUnitOwnerProfileResponse
(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
