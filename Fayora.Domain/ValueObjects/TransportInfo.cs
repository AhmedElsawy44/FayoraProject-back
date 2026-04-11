namespace Fayora.Domain.ValueObjects;

public record TransportInfo(
    bool HasOwnVehicle,
    string? VehicleDetails,
    string? TransportType
    );
