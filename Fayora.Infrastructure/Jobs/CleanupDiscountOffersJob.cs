using Fayora.Application.Common.Interfaces.Persistences.SharedModule;

namespace Fayora.Infrastructure.Jobs
{
    public class CleanupDiscountOffersJob(IDiscountOfferRepository discountOfferRepository)
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await discountOfferRepository.CleanUpExpiredOrCancelledOffersAsync(cancellationToken);
        }
    }
}
