using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TouristModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction
{
    public record TrackUserInteractionCommand(
        Guid EntityId,
        EntityType EntityType,
        InteractionType InteractionType
    ) : IRequest<Result<Success>>;
}
