using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class VerificationCodeRepository(ApplicationDbContext context) : BaseRepository<VerificationCode, int>(context), IVerificationCodeRepository
{
    public async Task<VerificationCode?> GetUserCodeAsync(
    Guid userId,
    string identifier,
    OtpPurpose purpose,
    CancellationToken cancellationToken,
    bool isTracking = true)
    {
        var query = context.VerificationCodes.AsQueryable();

        if (!isTracking) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(v =>
            v.UserId == userId &&
            v.Target == identifier &&
            v.Purpose == purpose &&
            !v.IsRevoked &&
            !v.IsUsed,
            cancellationToken);
    }
}
