using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public record CreateTourGuideCommand
(
    string DeviceId,
    string ProfessionalLicenseUrl
) : ICommand<Result<CreateTourGuideResult>>;
