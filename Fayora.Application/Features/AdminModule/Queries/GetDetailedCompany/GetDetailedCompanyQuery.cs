using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedCompany;

public record GetDetailedCompanyQuery(Guid Id)
    : IQuery<Result<GetDetailedCompanyResponse>>;
