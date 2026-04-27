namespace Fayora.Contracts.TourGuideModule.CreateTourCompany;

public record CreateTourCompanyResponse
(
    Guid UserId,
    string CompanyName,
    string Status,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
