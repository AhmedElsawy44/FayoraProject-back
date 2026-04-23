namespace Fayora.Contracts.TourGuideModule.CreateTourGuide;

public record CreateTourGuideResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Status,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
