using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.ValueObjects
{
    public record TransportInfo(
        bool HasOwnVehicle,
        string? VehicleDetails,
        string? TransportType
        );
}
