using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.CreateCompanyPackage
{
    public record CreateCompanyPackageResponse(
        Guid PackageId,
        Guid CompanyId,
        string Title,
        string Status,
        DateTimeOffset CreatedAt);
}
