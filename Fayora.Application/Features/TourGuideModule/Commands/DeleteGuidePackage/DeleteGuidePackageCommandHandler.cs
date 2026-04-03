using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage
{
    public class DeleteGuidePackageCommandHandler(
        ITourGuideRepository tourGuideRepository,
        ITourGuidePackageRepository tourGuidePackageRepository,
        IUnitOfWork unitOfWork,
        IClientContextProvider clientContextProvider) : IRequestHandler<DeleteGuidePackageCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteGuidePackageCommand request, CancellationToken cancellationToken)
        {
            var guideId = clientContextProvider.GetContext().TourGuideId;

            if (guideId is null) return TourGuideErrors.GuidIdNotExist;

            var guideExists = await tourGuideRepository.ExistsAsync(guideId.Value, cancellationToken);

            if (guideExists is false) return TourGuideErrors.GuideNotFound;

            var package = await tourGuidePackageRepository.GetPackageByIdAsync(request.PackageId, false, cancellationToken);

            if (package is null) return TourGuideErrors.PackageNotFound;

            if (package.GuideId != guideId.Value) return TourGuideErrors.UnauthorizedPackageModification;

            tourGuidePackageRepository.DeletePackage(package);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
