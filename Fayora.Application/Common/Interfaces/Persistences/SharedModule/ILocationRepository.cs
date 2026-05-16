using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule
{
    public interface ILocationRepository
    {
        void AddLocation(Location location);
        Task<Location?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<(List<Location> Items, int TotalCount)> GetAllLocationsAsync(
            LocationCategory? category,
            decimal? minRating,
            string? search,
            int page,
            int pageSize,
            CancellationToken ct = default);

        Task<bool> LocationExistsAsync(int id, CancellationToken cancellationToken = default);
        void RemoveLocation(Location location);
    }
}
