using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Queries.GetAllCompanyPackages
{
    using Fayora.Application.Features.TourCompanyModule.Queries.GetCompanyPackages;
    using Fayora.Domain.Common.Results;
    using MediatR;


    public record GetCompanyPackagesQuery(Guid CompanyId) : IRequest<Result<List<GetCompanyPackagesResult>>>;
}
