using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;

public record ActivateGuidePackageCommand(Guid PackageId) : IRequest<Result<Unit>>;
