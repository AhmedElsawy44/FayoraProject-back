using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourCompanyModule.CreateTourCompany;

public record CreateTourCompanyRequest
(
    string CompanyName,
    string Description,
    string ProfilePictureUrl,
    string LicenseDocumentUrl,
    string LicenseClass
);
