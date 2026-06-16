namespace Fayora.Contracts.AdminModule.GetUsers;

public record CompanyPackageDto(
    Guid PackageId,
    string Title,
    decimal AdultPrice,
    decimal ChildPrice,
    string Status,
    int Views,
    int DurationHours,
    string MainImageUrl
);

public record GetDetailedCompanyResponse(
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string CompanyName,
    bool IsSuperCompany,
    string LicenseDocumentUrl,
    string LicenseClass,
    string CurrencyCode,
    decimal AverageRating,
    int ReviewCount,
    int CompletedToursCount,
    bool IsAvailableForBooking,
    int Views,
    decimal ResponseRate,
    decimal CancellationRate,
    string Status,
    DateTimeOffset CreatedAt,
    string? AdminNotes,
    List<CompanyPackageDto> AssociatedPackages
);
