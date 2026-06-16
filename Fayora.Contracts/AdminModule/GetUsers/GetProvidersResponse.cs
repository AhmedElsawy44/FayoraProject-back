namespace Fayora.Contracts.AdminModule.GetUsers;

public record GetProvidersResponse(
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Status,
    DateTimeOffset CreatedAt,
    decimal? Rating,
    int? TotalItemsCount
);
