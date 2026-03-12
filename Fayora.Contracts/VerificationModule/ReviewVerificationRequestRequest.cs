namespace Fayora.Contracts.VerificationModule
{
    public record ReviewVerificationRequestRequest(
    Guid AdminId,
    string NewStatus,
    string? AdminComment);
}
