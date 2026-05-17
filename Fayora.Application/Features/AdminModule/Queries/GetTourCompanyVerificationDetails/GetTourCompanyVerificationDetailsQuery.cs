using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourCompanyVerificationDetails;


public record GetTourCompanyVerificationDetailsQuery(Guid Id)
    : IQuery<Result<GetTourCompanyVerificationDetailsResponse>>;
