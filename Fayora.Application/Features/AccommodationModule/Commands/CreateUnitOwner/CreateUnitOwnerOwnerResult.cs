namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public record CreateUnitOwnerOwnerResult(
    Guid UserId,
    string Token,
    string RefreshToken,
    int ExpiresIn);
