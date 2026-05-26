using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetUsers;

public record GetUsersQuery(
    int PageNumber,
    int PageSize,
    string? SearchQuery,
    string? RoleFilter,
    string? StatusFilter) : IQuery<Result<List<GetUsersResponse>>>;
