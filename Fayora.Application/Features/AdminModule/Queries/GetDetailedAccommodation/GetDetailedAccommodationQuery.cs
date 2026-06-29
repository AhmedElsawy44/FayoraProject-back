using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.UpdateAccommodation;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedAccommodation;

public record GetDetailedAccommodationQuery(Guid Id)
    : IQuery<Result<GetDetailedAccommodationResponse>>;
