using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;

public record ReviewVerificationRequestCommand(
    int RequestId,
    RequestStatus NewStatus,
    string? AdminComment) : ICommand<Result<ReviewVerificationRequestResult>>;
