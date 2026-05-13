using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails
{
    public record GetPackageDetailsQuery(Guid PackageId) : IQuery<Result<PackageDetailsResult>>;
}
