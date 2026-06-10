using Fayora.Domain.Entities.GuideModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule
{
    public interface IPackageNightRepository
    {
        void Add(PackageNight night);
        Task<List<PackageNight>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    }

}
