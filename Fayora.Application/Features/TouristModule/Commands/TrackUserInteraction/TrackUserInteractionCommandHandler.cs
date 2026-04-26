using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.TouristModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TouristModule;
using Fayora.Domain.Enums.TouristModule;
using MediatR;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction;

public class TrackUserInteractionCommandHandler
    (
       IUserInteractionRepository userInteractionRepository,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork,
       IMediator mediator
    )
    : ICommandHandler<TrackUserInteractionCommand, Result<Success>>
{

    public async Task<Result<Success>> Handle(
    TrackUserInteractionCommand request,
    CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();

        if (context.UserId == Guid.Empty)
            return Error.Unauthorized("UserId is required to track user interaction.");

        
        var interaction = new UserInteraction(
            context.UserId,
            request.EntityId,
            request.EntityType,
            request.InteractionType
        );
        userInteractionRepository.AddInteraction(interaction);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        
        if (request.InteractionType == InteractionType.View)
        {
            await mediator.Publish(new EntityViewedEvent(
                request.EntityId,
                request.EntityType
            ), cancellationToken);
        }

        return Result.Success;
    }
}