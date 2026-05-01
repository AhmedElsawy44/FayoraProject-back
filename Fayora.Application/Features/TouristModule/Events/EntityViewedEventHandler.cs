using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Events.TouristModule;
using Fayora.Domain.Enums.TouristModule;
using MediatR;

namespace Fayora.Application.Features.TouristModule.Events
{
    public class EntityViewedEventHandler(
        IPackageRepository packageRepository,
        ITourGuideRepository tourGuideRepository,
        ITourCompanyRepository tourCompanyRepository,
        IHousingUnitRepository housingUnitRepository,
        IUnitOfWork unitOfWork
    ) : INotificationHandler<EntityViewedEvent>
    {
        public async Task Handle(EntityViewedEvent notification, CancellationToken cancellationToken)
        {
            switch (notification.EntityType)
            {
                case EntityType.Package:
                    var package = await packageRepository.GetPackageByIdAsync(
                        notification.EntityId,
                        new() { ReadOnly = false },
                        cancellationToken);
                    package?.IncrementViews();
                    break;

                case EntityType.TourGuide:
                    var guide = await tourGuideRepository.GetGuideByIdAsync(
                        notification.EntityId,
                        new() { ReadOnly = false },
                        cancellationToken);
                    guide?.IncrementViews();
                    break;

                case EntityType.Company:
                    var company = await tourCompanyRepository.GetTourCompanyByIdAsync(
                        notification.EntityId,
                        new() { ReadOnly = false },
                        cancellationToken);
                    company?.IncrementViews();
                    break;

                case EntityType.Accommodation:
                    var unit = await housingUnitRepository.GetUnitByIdAsync(
                        notification.EntityId,
                        new() { IsReadOnly = false },
                        cancellationToken);
                    unit?.IncrementViews();
                    break;
            }

            await unitOfWork.CommitChangesAsync(cancellationToken);
        }
    }
}
