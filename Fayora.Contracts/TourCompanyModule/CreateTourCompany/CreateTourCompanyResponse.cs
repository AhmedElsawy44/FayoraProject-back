using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.CreateTourCompany
{
    public record CreateTourCompanyResponse(
        Guid CompanyId,
        Guid UserId,
        string CompanyName,
        string Status,
        string Message,
        DateTimeOffset CreatedAt);
}
