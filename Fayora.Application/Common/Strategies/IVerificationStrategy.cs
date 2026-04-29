using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Strategies;

public interface IVerificationStrategy
{
    bool CanHandle(string entityType);
    Task<Result<Success>> ProcessVerificationAsync(Guid entityId, bool isApproved, string adminNotes, CancellationToken ct);
}
