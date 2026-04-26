using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateTourGuide;

public record UpdateTourGuideCommand(
    int YearsOfExperience,
    PricingUnit PricingUnit,
    decimal BaseRate,
    List<int> CoveredCities
    ) : ICommand<Result<Guid>>;

