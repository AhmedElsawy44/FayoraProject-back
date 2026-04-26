using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Domain.Common.Events.TouristModule;
using Fayora.Domain.Entities.TouristModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Events.TouristModule
{
    public class UserInteractionEventHandler
        (
         IUserInteractionRepository userInteractionRepository,
         IUnitOfWork unitOfWork) : 
        INotificationHandler<UserInteractionEvent>
    {
        public async Task Handle(UserInteractionEvent notification, CancellationToken cancellationToken)
        {
            var interaction = new UserInteraction(
                notification.UserId,
                notification.EntityId,
                notification.EntityType,
                notification.InteractionType
                );
            await userInteractionRepository.AddInteractionAsync(interaction, cancellationToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);
        }
    }
}
