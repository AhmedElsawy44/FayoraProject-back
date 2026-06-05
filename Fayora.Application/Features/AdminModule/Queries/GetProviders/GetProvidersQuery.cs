using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetProviders;

public record GetProvidersQuery(
    string ProviderType,
    int PageNumber,
    int PageSize,
    string? SearchQuery
) : IQuery<Result<List<GetProvidersResponse>>>;
