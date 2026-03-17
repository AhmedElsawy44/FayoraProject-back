using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Api.Requests
{
    public record ReviewVerificationRequestRequest(
    Guid AdminId,
    RequestStatus NewStatus,
    string? AdminComment);
}
