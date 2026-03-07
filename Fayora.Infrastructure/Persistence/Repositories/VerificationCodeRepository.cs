using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class VerificationCodeRepository(ApplicationDbContext context) : IVerificationCodeRepository
{
    public Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, CodePurpose purpose, CancellationToken cancellationToken = default, bool isReadOnly = true)
    {
        var query = context.VerificationCodes.AsQueryable();

        if (isReadOnly) query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(v => v.UserId == userId && v.Target == identifier && v.Purpose == purpose, cancellationToken);
    }
}
