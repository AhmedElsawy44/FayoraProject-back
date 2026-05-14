using Fayora.Domain.Entities.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule
{
    public interface ILocationRepository
    {
        void AddLocation(Location location);
        Task<Location?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken = default);
        Task<bool> LocationExistsAsync(int id, CancellationToken cancellationToken = default);
        void RemoveLocation(Location location);
    }
}
