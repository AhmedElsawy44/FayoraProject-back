using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage
{
    public class ActivateGuidePackageCommandHandler(
       // ITourGuideRepository tourGuideRepository,
        ITourGuidePackageRepository tourGuidePackageRepository,
        IUnitOfWork unitOfWork,
        IClientContextProvider clientContextProvider) : IRequestHandler<ActivateGuidePackageCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(ActivateGuidePackageCommand request, CancellationToken cancellationToken)
        {
            var guideId = clientContextProvider.GetContext().TourGuideId;

            if (guideId is null) return TourGuideErrors.GuidIdNotExist;

            var package = await tourGuidePackageRepository.GetPackageByIdAsync(request.PackageId, false, cancellationToken);

            if (package is null) return TourGuideErrors.PackageNotFound;

            if (package.GuideId != guideId.Value) return TourGuideErrors.UnauthorizedPackageModification;

            if (package.IsActive is true) return TourGuideErrors.PackageIsAlreadyActivated;

            package.Activate();

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
