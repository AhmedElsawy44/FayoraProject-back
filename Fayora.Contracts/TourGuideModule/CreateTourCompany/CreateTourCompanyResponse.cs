namespace Fayora.Contracts.TourGuideModule.CreateTourCompany;

public record CreateTourCompanyResponse(
    Guid CompanyId,
    Guid UserId,
    string CompanyName,
    string Status,
    string Message,
    DateTimeOffset CreatedAt);
