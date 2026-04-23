namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;

public record CreateTourCompanyResult
(
    Guid UserId,
    string CompanyName,
    string Status,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);