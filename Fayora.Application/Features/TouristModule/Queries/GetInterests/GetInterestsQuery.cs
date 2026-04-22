using MediatR;

namespace Fayora.Application.Features.TouristModule.Queries.GetInterests;

public record GetInterestsQuery : IRequest<GetInterestsResult>;