using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TouristModule;
using MediatR;

namespace Fayora.Application.Features.Tourist.Commands.CreateTouristProfile;

public record CreateTouristProfileCommand(
    BudgetTier? BudgetTier,
    TravelStyle? TravelStyle,
    HashSet<int> InterestIds) : IRequest<Result<CreateTouristProfileResult>>;
