using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

public class VerificationCodeRepository(ApplicationDbContext context) : IVerificationCodeRepository
{
    public Task<VerificationCode?> GetUserCodeAsync(Guid userId, string target, CodePurpose purpose, CancellationToken cancellationToken, bool isReadOnly = true)
    {
        var query = context.VerificationCodes.AsQueryable();

        if (isReadOnly) query = query.AsNoTracking();

        return context.VerificationCodes
        .Where(x => x.UserId == userId
                 && x.Target == target
                 && x.Purpose == purpose)
        .OrderByDescending(x => x.CreatedAt)
        .FirstOrDefaultAsync(cancellationToken);
    }
}
