using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetVerificationQueue;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;

public record GetVerificationQueueQuery(
    PartnerTypeFilter Type,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<List<GetVerificationQueueResoponse>>;
