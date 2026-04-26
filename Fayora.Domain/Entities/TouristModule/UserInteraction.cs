using Fayora.Domain.Entities.IdentityModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.TouristModule
{
    public class UserInteraction
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid EntityId { get; private set; }
        public EntityType EntityType {  get; private set; }
        public InteractionType InteractionType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserInteraction() { } 

        public UserInteraction(
            Guid userId,
            Guid entityId,
            EntityType entityType,
            InteractionType interactionType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            EntityId = entityId;
            EntityType = entityType;
            InteractionType = interactionType;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public enum InteractionType
    {
        View = 1,
        Click = 2,
        Favorite = 3,
    }

    public enum EntityType
    {
        Package = 1,
        TourGuide = 2,
        Company = 3,
        Accommodation = 4,
    }
}
