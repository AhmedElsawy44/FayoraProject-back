using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Enums.SharedModule;

[Flags]
public enum TourType
{
    None = 0,
    Cultural = 1,
    Adventure = 2,
    Historical = 4,
    Nature = 8,
    Food = 16,
    Religious = 32,
    Beach = 64
}
