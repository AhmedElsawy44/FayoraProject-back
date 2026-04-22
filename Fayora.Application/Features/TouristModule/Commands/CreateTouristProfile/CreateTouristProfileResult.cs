namespace Fayora.Application.Features.TouristModule.Commands.CreateTouristProfile;

public record CreateTouristProfileResult(
    Guid TouristId,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
