using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourGuideVerificationDetails;

public record GetTourGuideVerificationDetailsQuery(Guid Id)
    : IQuery<Result<GetTourGuideVerificationDetailsResponse>>;