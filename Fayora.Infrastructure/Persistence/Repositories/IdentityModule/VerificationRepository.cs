using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
