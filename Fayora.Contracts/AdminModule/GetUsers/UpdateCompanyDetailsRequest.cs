namespace Fayora.Contracts.AdminModule.GetUsers;

public record UpdateCompanyDetailsRequest(
    string CompanyName,
    bool IsSuperCompany,
    string LicenseClass,
    decimal AverageRating,
    int ReviewCount,
    int CompletedToursCount,
    bool IsAvailableForBooking,
    decimal ResponseRate,
    decimal CancellationRate,
    string Status,
    string? AdminNotes
);
