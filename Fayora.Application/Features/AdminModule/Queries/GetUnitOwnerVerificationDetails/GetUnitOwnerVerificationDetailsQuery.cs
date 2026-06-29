using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetUnitOwnerVerificationDetails;

public record GetUnitOwnerVerificationDetailsQuery(Guid Id)
    : IQuery<Result<GetUnitOwnerVerificationDetailsResponse>>;
