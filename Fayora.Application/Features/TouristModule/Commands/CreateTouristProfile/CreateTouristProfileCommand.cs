using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TouristModule;

namespace Fayora.Application.Features.TouristModule.Commands.CreateTouristProfile;

public record CreateTouristProfileCommand(
    string DeviceId,
    BudgetTier? BudgetTier,
    TravelStyle? TravelStyle,
    HashSet<int>? InterestIds) : ICommand<Result<CreateTouristProfileResult>>;