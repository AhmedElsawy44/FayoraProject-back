using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class HousingUnitRepository(ApplicationDbContext context) : IHousingUnitRepository
{
    public void AddUnit(HousingUnit housingUnit)
    {
         context.HousingUnits.Add(housingUnit);
    }
}
