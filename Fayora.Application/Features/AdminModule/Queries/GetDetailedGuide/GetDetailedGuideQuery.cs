using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedGuide;

public record GetDetailedGuideQuery(Guid Id) 
    : IQuery<Result<GetDetailedGuideResponse>>;
