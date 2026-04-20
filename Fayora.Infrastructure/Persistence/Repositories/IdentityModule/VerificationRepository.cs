using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.SharedModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule
{
    public class VerificationRepository(ApplicationDbContext context) : IVerificationRepository
    {
        public async Task AddAsync(VerificationRequest request, CancellationToken ct = default)
            => await context.VerificationRequests.AddAsync(request, ct);

        public async Task<VerificationRequest?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.VerificationRequests
            .Include(r => r.VerificationDocuments)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<VerificationRequest?> GetByUserIdAndTypeAsync(
            Guid userId, RequestType requestType, CancellationToken ct = default)
            => await context.VerificationRequests
                .Include(r => r.VerificationDocuments)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RequestType == requestType, ct);
    }
}
