using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;

public class DeleteGuidePackageCommandHandler(
    ITourGuideRepository tourGuideRepository,
    ITourCompanyRepository tourCompanyRepository,
    IPackageRepository packageRepository,
    IPackageNightRepository packageNightRepository,
    IPackageAccommodationRepository packageAccommodationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : IRequestHandler<DeleteGuidePackageCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();

        var guideId = context.UserId;

        var roles = context.Roles;

        GuideAccountBase? guide = null;
        if (roles is not null)
        {
            if (roles.Contains(Role.TourGuide.ToString()))
                guide = await tourGuideRepository.GetGuideByIdAsync(guideId, new GuideQueryOptions { ReadOnly = false }, cancellationToken);
            else if (roles.FirstOrDefault(r => r == Role.TourCompany.ToString()) is not null)
                guide = await tourCompanyRepository.GetTourCompanyByIdAsync(guideId, new GuideQueryOptions { ReadOnly = false }, cancellationToken);
        }

        if (guide is null) return TourGuideErrors.GuideNotFound;
        var result = guide.DeletePackage(request.PackageId);
        if (result.IsError) return result.Errors;

        var package = await packageRepository.GetPackageByIdAsync(request.PackageId, new PackageQueryOptions { ReadOnly = false }, cancellationToken);
        if (package is null) return TourGuideErrors.PackageNotFound;

        package.Delete();

        // Delete associated nights and accommodations
        var existingNights = await packageNightRepository.GetByPackageIdAsync(request.PackageId, cancellationToken);
        if (existingNights.Any())
            packageNightRepository.RemoveNights(existingNights);

        var existingAccommodations = await packageAccommodationRepository.GetByPackageIdAsync(request.PackageId, cancellationToken);
        if (existingAccommodations.Any())
            packageAccommodationRepository.RemoveAccommodations(existingAccommodations);


        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
