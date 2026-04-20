using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;

public record GetVerificationRequestQuery(int RequestId) : IQuery<Result<GetVerificationRequestResponse>>;
