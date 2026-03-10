using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

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
