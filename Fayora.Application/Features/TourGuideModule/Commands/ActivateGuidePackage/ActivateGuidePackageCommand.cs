using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;

public record ActivateGuidePackageCommand(Guid PackageId) : ICommand<Result<Unit>>;
