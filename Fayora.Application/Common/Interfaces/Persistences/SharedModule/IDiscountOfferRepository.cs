using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule
{
    public interface IDiscountOfferRepository
    {
        void Add(DiscountOffer offer);

        Task<DiscountOffer?> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default);

        Task<List<DiscountOffer>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

        Task<List<DiscountOffer>> GetAllOffersAsync(CancellationToken cancellationToken = default);

        Task<List<DiscountOffer>> GetActiveByTargetAsync(
            Guid targetId,
            OfferTargetType targetType,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveOfferForTargetAsync(
            Guid targetId,
            OfferTargetType targetType,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            CancellationToken cancellationToken = default);

        Task<DiscountOffer?> GetActiveByCodeAndTargetAsync(
            string code,
            Guid targetId,
            OfferTargetType targetType,
            CancellationToken cancellationToken = default);

        Task CleanUpExpiredOrCancelledOffersAsync(CancellationToken cancellationToken = default);
    }
}
