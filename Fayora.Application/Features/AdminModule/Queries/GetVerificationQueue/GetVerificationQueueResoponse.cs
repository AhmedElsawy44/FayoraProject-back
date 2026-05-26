using Fayora.Contracts.AdminModule.GetVerificationQueue;

namespace Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;

public record GetVerificationQueueResoponse(
    Guid Id,
    string Name,
    PartnerTypeFilter Type,
    int DocsCount,
    DateTime SubmittedAt
);
