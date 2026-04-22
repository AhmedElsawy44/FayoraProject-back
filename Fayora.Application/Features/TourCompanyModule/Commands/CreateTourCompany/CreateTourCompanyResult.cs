namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany
{
    public record CreateTourCompanyResult(
        Guid CompanyId,
        Guid UserId,
        string CompanyName,
        string Status,
        string Message,
        DateTimeOffset CreatedAt);
}