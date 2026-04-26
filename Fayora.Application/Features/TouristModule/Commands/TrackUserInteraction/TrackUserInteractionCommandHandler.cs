using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.TouristModule;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction
{
    public class TrackUserInteractionCommandHandler
        (
            IMediator mediator,
            IClientContextProvider clientContextProvider
        )
        : IRequestHandler<TrackUserInteractionCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            TrackUserInteractionCommand request,
            CancellationToken cancellationToken)
        {
            var context = clientContextProvider.GetContext();

           if (context.UserId == Guid.Empty)
              return Error.Unauthorized("UserId is required to track user interaction.");

            await mediator.Publish(new UserInteractionEvent(
                context.UserId,
                request.EntityId,
                request.EntityType,
                request.InteractionType
            ), cancellationToken);

            return Result.Success;
        }
    }
}