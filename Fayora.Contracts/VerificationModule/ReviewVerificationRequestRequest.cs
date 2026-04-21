namespace Fayora.Contracts.VerificationModule;

public record ReviewVerificationRequestRequest(
string NewStatus,
string? AdminComment);
