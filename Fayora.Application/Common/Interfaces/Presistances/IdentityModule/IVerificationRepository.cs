using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule
{
    public interface IVerificationRepository
    {
        Task AddAsync(VerificationRequest request, CancellationToken ct = default);
        Task<VerificationRequest?> GetByUserIdAndTypeAsync(Guid userId, RequestType requestType, CancellationToken ct = default);

        Task<VerificationRequest?> GetByIdAsync(int id, CancellationToken ct = default);

    }
}
