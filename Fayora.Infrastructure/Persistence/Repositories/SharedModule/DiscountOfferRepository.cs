using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.SharedModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule
{
    public class DiscountOfferRepository(ApplicationDbContext context) : IDiscountOfferRepository
    {
        public void Add(DiscountOffer offer)
        {
            context.DiscountOffers.Add(offer);
        }

        public async Task<DiscountOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<List<DiscountOffer>> GetActiveByTargetAsync(
            Guid targetId,
            OfferTargetType targetType,
            CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .AsNoTracking()
                .Where(o => o.TargetId == targetId
                         && o.TargetType == targetType
                         && o.Status == DiscountOfferStatus.Active
                         && o.StartDate <= DateTimeOffset.UtcNow
                         && o.EndDate > DateTimeOffset.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DiscountOffer>> GetByOwnerIdAsync(
            Guid ownerId,
            CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .AsNoTracking()
                .Where(o => o.OwnerId == ownerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // check if the target has an active offer in the same date range
        public async Task<bool> HasActiveOfferForTargetAsync(
            Guid targetId,
            OfferTargetType targetType,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .AnyAsync(o =>
                    o.TargetId == targetId &&
                    o.TargetType == targetType &&
                    o.Status == DiscountOfferStatus.Active &&
                    o.StartDate < endDate &&
                    o.EndDate > startDate,
                    cancellationToken);
        }

        public async Task CleanUpExpiredOrCancelledOffersAsync(CancellationToken cancellationToken = default)
        {
            // Mark active offers whose end dates have passed as Expired
            await context.DiscountOffers
                .Where(o => o.Status == DiscountOfferStatus.Active && o.EndDate < DateTimeOffset.UtcNow)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, DiscountOfferStatus.Expired), cancellationToken);

            // Identify offers that are Cancelled or Expired, excluding those with active or future bookings
            var nowUtc = DateTime.UtcNow;
            var offerIdsWithActiveBookings = await context.Bookings
                .Where(b => b.AppliedOfferId != null && b.EndDate >= nowUtc)
                .Select(b => b.AppliedOfferId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var offersToDelete = await context.DiscountOffers
                .Where(o => (o.Status == DiscountOfferStatus.Expired || o.Status == DiscountOfferStatus.Cancelled)
                            && !offerIdsWithActiveBookings.Contains(o.Id))
                .ToListAsync(cancellationToken);

            if (offersToDelete.Any())
            {
                var offerIdsToDelete = offersToDelete.Select(o => o.Id).ToList();

                // Null out AppliedOfferId in Bookings referencing these offers
                await context.Bookings
                    .Where(b => b.AppliedOfferId != null && offerIdsToDelete.Contains(b.AppliedOfferId.Value))
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.AppliedOfferId, (Guid?)null), cancellationToken);

                // Delete the offers from database
                await context.DiscountOffers
                    .Where(o => offerIdsToDelete.Contains(o.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }

        public async Task<DiscountOffer?> GetActiveByCodeAndTargetAsync(
            string code,
            Guid targetId,
            OfferTargetType targetType,
            CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .FirstOrDefaultAsync(o => o.TargetId == targetId
                                       && o.TargetType == targetType
                                       && o.Status == DiscountOfferStatus.Active
                                       && o.Title.ToLower() == code.ToLower()
                                       && o.StartDate <= DateTimeOffset.UtcNow
                                       && o.EndDate > DateTimeOffset.UtcNow,
                                     cancellationToken);
        }
    }
}
