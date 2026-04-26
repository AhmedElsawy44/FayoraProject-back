namespace Fayora.Contracts.TouristModule;

public record TrackUserInteractionRequest(
    Guid EntityId,
    EntityTypeDto EntityType,
    InteractionTypeDto InteractionType
);

public enum InteractionTypeDto
{
    View = 1,
    Click = 2,
    Favorite = 3,
}

public enum EntityTypeDto
{
    Package = 1,
    TourGuide = 2,
    Company = 3,
    Accommodation = 4,
}
