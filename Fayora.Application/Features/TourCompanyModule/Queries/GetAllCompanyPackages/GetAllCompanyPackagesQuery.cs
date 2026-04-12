using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Queries.GetAllCompanyPackages
{
    using Fayora.Domain.Common.Results;
    using MediatR;


    public record GetAllCompanyPackagesQuery(Guid CompanyId) : IRequest<Result<List<GetCompanyPackagesResult>>>;
}
