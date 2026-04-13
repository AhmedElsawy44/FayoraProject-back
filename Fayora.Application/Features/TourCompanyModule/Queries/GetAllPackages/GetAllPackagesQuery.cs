using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.Queries.GetAllPackages
{
    public record GetAllPackagesQuery(
        TourType? TourType,
        decimal? MinPrice,
        decimal? MaxPrice,
        DateOnly? StartDate,
        DateOnly? EndDate,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<Result<GetAllPackagesResult>>;
}
