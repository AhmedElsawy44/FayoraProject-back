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

        public async Task<bool> HasActiveOfferForTargetAsync(
            Guid targetId,
            OfferTargetType targetType,
            CancellationToken cancellationToken = default)
        {
            return await context.DiscountOffers
                .AnyAsync(o =>
                    o.TargetId == targetId &&
                    o.TargetType == targetType &&
                    o.Status == DiscountOfferStatus.Active &&
                    o.EndDate > DateTimeOffset.UtcNow,
                    cancellationToken);
        }

    }


}
