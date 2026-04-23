namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public record CreateUnitOwnerOwnerResult
(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
