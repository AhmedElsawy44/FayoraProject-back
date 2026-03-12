using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TouristModule;
using MediatR;

namespace Fayora.Application.Features.Tourist.Commands.CreateTouristProfile;

public record CreateTouristProfileCommand(
    string DeviceId,
    BudgetTier? BudgetTier,
    TravelStyle? TravelStyle,
    HashSet<int> InterestIds) : IRequest<Result<CreateTouristProfileResult>>, ICheckBannedRequest;