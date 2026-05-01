using Fayora.Domain.Entities.GuideModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule
{
    public interface IPackageOccurrenceRepository
    {
        Task AddRangeAsync(List<PackageOccurrence> occurrences, CancellationToken cancellationToken);
        Task<bool> HasOverlappingOccurrenceAsync(Guid packageId, List<DateTime> dates, CancellationToken cancellationToken);
        Task<bool> PackageExistsForUserAsync(Guid packageId, Guid userId, CancellationToken cancellationToken);
    }
}
