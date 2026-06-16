using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.UpdateLocation;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedLocation;

public record GetDetailedLocationQuery(int Id) 
    : IQuery<Result<GetDetailedLocationResponse>>;
