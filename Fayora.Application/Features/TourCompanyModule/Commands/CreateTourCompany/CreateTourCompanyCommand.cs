using Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.CreateTourCompany
{
    public record CreateTourCompanyCommand(
        Guid UserId,
        string CompanyName,
        string Description,
        string CommercialRegisterNumber,
        string TaxRegistrationNumber,
        string CurrencyCode,
        string? LogoUrl
    ) : IRequest<Result<CreateTourCompanyResult>>;
}
