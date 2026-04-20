using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Enums.SharedModule;
namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest
{
    public record ReviewVerificationRequestCommand(
        int RequestId,
        Guid AdminId,
        RequestStatus NewStatus,
        string? AdminComment) : ICommand<Result<ReviewVerificationRequestResponse>>;
}
