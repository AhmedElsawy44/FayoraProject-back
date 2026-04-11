using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Common.Interfaces.Persistences.IdentityModule
{
    public interface IVerificationRepository
    {
        Task AddAsync(VerificationRequest request, CancellationToken ct = default);
        Task<VerificationRequest?> GetByUserIdAndTypeAsync(Guid userId, RequestType requestType, CancellationToken ct = default);

        Task<VerificationRequest?> GetByIdAsync(int id, CancellationToken ct = default);

    }
}
