using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.GetTourCompanyById
{
    public record GetTourCompanyByIdResponse(
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
