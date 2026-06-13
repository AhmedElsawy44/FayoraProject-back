using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Events.ReviewModule;
using Fayora.Domain.Enums.ReviewModule;
using MediatR;

namespace Fayora.Application.Features.ReviewModule.Events;

public class ReviewDeletedEventHandler(
    ITourGuideRepository tourGuideRepository,
    IPackageRepository packageRepository,
    IHousingUnitRepository housingUnitRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<ReviewDeletedEvent>
{
    public async Task Handle(ReviewDeletedEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.TargetType)
        {
            case ReviewTargetType.TourGuide:
                var guide = await tourGuideRepository.GetGuideByIdAsync(
                    notification.TargetId,
                    new ITourGuideRepository.GuideQueryOptions(ReadOnly: false),
                    cancellationToken);
                if (guide is not null)
                {
                    guide.DeleteReview(notification.Rating);
                }
                break;

            case ReviewTargetType.GuidePackage:
                var package = await packageRepository.GetPackageByIdAsync(
                    notification.TargetId,
                    new IPackageRepository.PackageQueryOptions(ReadOnly: false),
                    cancellationToken);
                if (package is not null)
                {
                    package.DeleteReview(notification.Rating);
                }
                break;

            case ReviewTargetType.HousingUnit:
                var unit = await housingUnitRepository.GetUnitByIdAsync(
                    notification.TargetId,
                    new IHousingUnitRepository.UnitQueryOptions(IsReadOnly: false),
                    cancellationToken);
                if (unit is not null)
                {
                    unit.DeleteReview(notification.Rating);
                }
                break;
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}
