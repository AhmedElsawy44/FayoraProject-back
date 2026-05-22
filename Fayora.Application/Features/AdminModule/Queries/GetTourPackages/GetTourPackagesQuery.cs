using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.UpdateTourPackage;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourPackages;

public record GetTourPackagesQuery(
    int PageNumber,
    int PageSize,
    string? SearchQuery,
    string? StatusFilter) : IQuery<Result<List<GetTourPackagesResponse>>>;
