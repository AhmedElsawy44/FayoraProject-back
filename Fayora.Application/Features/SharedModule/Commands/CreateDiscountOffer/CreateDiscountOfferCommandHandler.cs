using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Errors;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Application.Features.SharedModule.Commands.CreateDiscountOffer
{
    public class CreateDiscountOfferCommandHandler(
        IClientContextProvider clientContextProvider,
        IDiscountOfferRepository discountOfferRepository,
        IHousingUnitRepository housingUnitRepository,
        IPackageRepository packageRepository,
        ITourGuideRepository tourGuideRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateDiscountOfferCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            CreateDiscountOfferCommand request,
            CancellationToken cancellationToken)
        {
            var ownerId = clientContextProvider.GetContext().UserId;

            // Verify existence of target and ownership/authorization
            switch (request.TargetType)
            {
                case OfferTargetType.HousingUnit:
                    var unit = await housingUnitRepository.GetUnitByIdAsync(
                        request.TargetId,
                        new UnitQueryOptions { IsReadOnly = true },
                        cancellationToken);
                    if (unit is null)
                        return DiscountOfferErrors.TargetNotFound;
                    if (unit.OwnerId != ownerId)
                        return DiscountOfferErrors.Unauthorized;
                    break;

                case OfferTargetType.GuidePackage:
                    var package = await packageRepository.GetPackageByIdAsync(
                        request.TargetId,
                        new PackageQueryOptions { ReadOnly = true },
                        cancellationToken);
                    if (package is null)
                        return DiscountOfferErrors.TargetNotFound;
                    if (package.UserId != ownerId)
                        return DiscountOfferErrors.Unauthorized;
                    break;

                case OfferTargetType.TourGuide:
                    var guide = await tourGuideRepository.GetGuideByIdAsync(
                        request.TargetId,
                        new GuideQueryOptions { ReadOnly = true },
                        cancellationToken);
                    if (guide is null)
                        return DiscountOfferErrors.TargetNotFound;
                    if (guide.UserId != ownerId)
                        return DiscountOfferErrors.Unauthorized;
                    break;

                default:
                    return DiscountOfferErrors.TargetNotFound;
            }

            //Check that this target doesn't have overlapping active offers
            var hasActive = await discountOfferRepository.HasActiveOfferForTargetAsync(
                request.TargetId,
                request.TargetType,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            if (hasActive)
                return DiscountOfferErrors.ActiveOfferAlreadyExists;

            var offerResult = DiscountOffer.Create(
                ownerId,
                request.TargetId,
                request.TargetType,
                request.Title,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.StartDate,
                request.EndDate);

            if (offerResult.IsError) return offerResult.Errors;

            discountOfferRepository.Add(offerResult.Value);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return offerResult.Value.Id.ToString();
        }
    }
}
