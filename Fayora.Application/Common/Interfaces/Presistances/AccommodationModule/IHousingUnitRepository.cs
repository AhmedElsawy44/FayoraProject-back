using Fayora.Domain.Entities.AccommodationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IHousingUnitRepository
{
    void AddUnit(HousingUnit housingUnit);
}
