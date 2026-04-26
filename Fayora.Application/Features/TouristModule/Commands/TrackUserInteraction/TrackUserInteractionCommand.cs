using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TouristModule;
using MediatR;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction;

public record TrackUserInteractionCommand(
    Guid EntityId,
    EntityType EntityType,
    InteractionType InteractionType
) : IRequest<Result<Success>>;
