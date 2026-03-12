using Fayora.Domain.Enums.Shared;

namespace Fayora.Api.Requests
{
    public record ReviewVerificationRequestRequest(
    Guid AdminId,
    RequestStatus NewStatus,
    string? AdminComment);
}
