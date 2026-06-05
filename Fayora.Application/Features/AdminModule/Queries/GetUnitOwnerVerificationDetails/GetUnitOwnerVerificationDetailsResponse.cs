namespace Fayora.Application.Features.AdminModule.Queries.GetUnitOwnerVerificationDetails;

public record GetUnitOwnerVerificationDetailsResponse(
    Guid UserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string OwnerType,
    string? CommercialName,
    string VerificationStatus,
    DateTimeOffset? VerifiedAt
);
