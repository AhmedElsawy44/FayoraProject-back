using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.CreateTourCompany
{
    public record CreateTourCompanyRequest(
        Guid UserId,
        string CompanyName,
        string Description,
        string CommercialRegisterNumber,
        string TaxRegistrationNumber,
        string CurrencyCode,
        string? LogoUrl);
}
