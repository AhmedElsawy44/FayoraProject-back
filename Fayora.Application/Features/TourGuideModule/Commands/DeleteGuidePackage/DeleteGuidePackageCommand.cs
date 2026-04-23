 using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;

public record DeleteGuidePackageCommand(Guid PackageId) : IRequest<Result<Unit>>;
