namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public record CreateTourGuideResult
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Status,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);