using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.TourGuideModule.IPackageRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;

public class ActivateGuidePackageCommandHandler(
    IPackageRepository tourGuidePackageRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : IRequestHandler<ActivateGuidePackageCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ActivateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var guideId = clientContextProvider.GetContext().UserId;

        var package =
            await tourGuidePackageRepository.GetPackageByIdAsync(request.PackageId, new PackageQueryOptions { ReadOnly = false }, cancellationToken);

        if (package is null) return TourGuideErrors.PackageNotFound;

        if (package.UserId != guideId) return TourGuideErrors.UnauthorizedPackageModification;

        if (package.IsActive is true) return TourGuideErrors.PackageIsAlreadyActivated;

        var result = package.Activate();
        if (result.IsError) return result.Errors;

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
