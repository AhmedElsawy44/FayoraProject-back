using MediatR;

namespace Fayora.Application.Features.Tourist.Queries.GetInterests;

public record GetInterestsQuery : IRequest<GetInterestsResult>;