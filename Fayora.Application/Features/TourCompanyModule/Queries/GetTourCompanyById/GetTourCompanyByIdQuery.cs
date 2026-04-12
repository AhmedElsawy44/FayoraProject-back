using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Queries.GetTorCompanyById
{
    public record GetTourCompanyByIdQuery(Guid CompanyId) : IRequest<Result<GetTourCompanyByIdResult>>;
}
