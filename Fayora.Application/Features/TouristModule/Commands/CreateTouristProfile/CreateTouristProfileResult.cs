namespace Fayora.Application.Features.TouristModule.Commands.CreateTouristProfile;

public record CreateTouristProfileResult(
    Guid UserId,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);
