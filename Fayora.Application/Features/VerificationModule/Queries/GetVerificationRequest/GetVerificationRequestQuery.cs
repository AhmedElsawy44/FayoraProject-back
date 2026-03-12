using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Verification.Queries.GetVerificationRequest
{
    public record GetVerificationRequestQuery(int RequestId) : IRequest<Result<GetVerificationRequestResponse>>;
}
