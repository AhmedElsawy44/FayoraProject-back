using Fayora.Domain.Entitties.Tourist;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances.TouristModule;

public interface ITouristRepository
{
    public void AddTourist(TouristProfile touristProfile);
}
