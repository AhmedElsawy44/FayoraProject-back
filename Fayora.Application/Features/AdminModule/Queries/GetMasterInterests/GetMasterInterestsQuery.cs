using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.MasterInterests;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetMasterInterests;

public record GetMasterInterestsQuery() : IQuery<Result<List<GetMasterInterestsResponse>>>;
