using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using MediatR;

namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest
{
    public record ReviewVerificationRequestCommand(
        int RequestId,
        Guid AdminId,
        RequestStatus NewStatus,
        string? AdminComment) : IRequest<Result<ReviewVerificationRequestResponse>>;
}
