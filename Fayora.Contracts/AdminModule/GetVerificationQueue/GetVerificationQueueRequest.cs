namespace Fayora.Contracts.AdminModule.GetVerificationQueue;

public enum PartnerTypeFilter
{
    All = 0,
    Guide = 1,
    Company = 2
}

public record GetVerificationQueueRequest(
    PartnerTypeFilter Type = PartnerTypeFilter.All,
    int PageNumber = 1,
    int PageSize = 10
);
