namespace Fayora.Contracts.TourGuideModule.CreateTourCompany;

public record CreateTourCompanyRequest
(
    string CompanyName,
    string Description,
    string ProfilePictureUrl,
    string LicenseDocumentUrl,
    string LicenseClass
);
