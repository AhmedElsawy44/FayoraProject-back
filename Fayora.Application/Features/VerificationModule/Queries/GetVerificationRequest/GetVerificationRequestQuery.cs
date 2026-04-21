using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;

public record GetVerificationRequestQuery(int RequestId) : IQuery<Result<GetVerificationRequestResponse>>;
