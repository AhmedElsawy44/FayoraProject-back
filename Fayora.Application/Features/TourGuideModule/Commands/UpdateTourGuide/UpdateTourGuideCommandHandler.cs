using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateTourGuide;

public class UpdateTourGuideCommandHandler(
    ITourGuideRepository tourGuideRepository,
    ICityRepository cityRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UpdateTourGuideCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateTourGuideCommand request, CancellationToken cancellationToken)
    {
        if(await cityRepository.CitiesExistsAsync(request.CoveredCities, cancellationToken) is false)
        {
            return TourGuideErrors.CitiesNotExist;
        }

        var userId = clientContextProvider.GetContext().UserId;

        var tourGuide = await tourGuideRepository.GetGuideByIdAsync(userId, new GuideQueryOptions { ReadOnly = false }, cancellationToken);

        if (tourGuide is null) return TourGuideErrors.GuideNotFound;

        var updateResult = tourGuide.Update(request.YearsOfExperience, request.PricingUnit, request.BaseRate);
        if (updateResult.IsError) return updateResult.Errors;

        var guideCities = request.CoveredCities.Select(cityId => new GuideCity(userId, cityId)).ToList();

        tourGuide.UpdateCities(guideCities);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return tourGuide.UserId;
    }
}
