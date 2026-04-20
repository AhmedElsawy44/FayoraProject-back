using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;

public record DeactivateGuidePackageCommand(Guid PackageId) : IRequest<Result<Unit>>;

