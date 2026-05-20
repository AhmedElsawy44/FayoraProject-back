using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Entities.SharedModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule
{
    public class LocationImageRepository(ApplicationDbContext context) : ILocationImageRepository
    {
        public void AddLocationImages(List<LocationImage> images)
            => context.LocationImages.AddRange(images);

        public async Task<List<LocationImage>> GetImagesByLocationIdAsync(
    int locationId,
    CancellationToken cancellationToken = default)
        {
            return await context.LocationImages
                .AsNoTracking()
                .Where(i => i.LocationId == locationId)
                .ToListAsync(cancellationToken);
        }
    }
}
