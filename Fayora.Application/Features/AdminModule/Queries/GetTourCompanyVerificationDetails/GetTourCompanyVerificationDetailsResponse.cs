namespace Fayora.Application.Features.AdminModule.Queries.GetTourCompanyVerificationDetails;

public record GetTourCompanyVerificationDetailsResponse(
    Guid UserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string CompanyName,
    string LicenseClass,
    string LicenseDocumentUrl
);
