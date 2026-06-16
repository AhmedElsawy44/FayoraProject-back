using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;

public record GetUnitByIdQuery(Guid UnitId) : IRequest<Result<GetUnitByIdResult>>;
