using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.Commands.DeleteCompanyPackage
{
    public record DeleteCompanyPackageCommand(Guid PackageId) : IRequest<Result<Success>>;
}
