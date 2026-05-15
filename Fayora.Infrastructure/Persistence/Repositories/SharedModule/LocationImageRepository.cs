using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Entities.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule
{
    public class LocationImageRepository(ApplicationDbContext context) : ILocationImageRepository
    {
        public void AddLocationImages(List<LocationImage> images)
            => context.LocationImages.AddRange(images);
    }
}
