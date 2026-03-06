    using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class VerificationCodeRepository(ApplicationDbContext context) : IVerificationCodeRepository
{
    public Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, OtpPurpose purpose, CancellationToken cancellationToken = default, bool isTracking = true)
    {
        var query = context.VerificationCodes.AsQueryable();

        if (!isTracking) query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(v => v.UserId == userId && v.Target == identifier && v.Purpose == purpose, cancellationToken);
    }
}
