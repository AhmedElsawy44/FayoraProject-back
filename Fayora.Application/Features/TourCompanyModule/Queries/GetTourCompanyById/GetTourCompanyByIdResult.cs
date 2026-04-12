namespace Fayora.Application.Features.TourCompanyModule.Queries.GetTorCompanyById
{
    public record GetTourCompanyByIdResult(
        Guid CompanyId,
        Guid UserId,
        string CompanyName,
        string Description,
        string CommercialRegisterNumber,
        string TaxRegistrationNumber,
        string CurrencyCode,
        string? LogoUrl,
        float Rating,
        int ReviewCount,
        int CompletedToursCount,
        string Status,
        bool IsListingEnabled,
        DateTimeOffset CreatedAt);
}