using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TouristModule;
using MediatR;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction;

public class TrackUserInteractionCommandHandler
    (
       IUserInteractionRepository userInteractionRepository,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork
    )
    : IRequestHandler<TrackUserInteractionCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        TrackUserInteractionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var interaction = new UserInteraction(
            userId,
            request.EntityId,
            request.EntityType,
            request.InteractionType
        );

        userInteractionRepository.AddInteraction(interaction);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}