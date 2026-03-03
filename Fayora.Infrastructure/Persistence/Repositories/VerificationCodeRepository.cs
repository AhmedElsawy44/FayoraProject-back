using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class VerificationCodeRepository(ApplicationDbContext context) : BaseRepository<VerificationCode, int>(context), IVerificationCodeRepository
{
    public Task<VerificationCode?> GetUserCode(Guid userId, string target, OtpPurpose purpose, CancellationToken cancellationToken, bool isTracking = false) => GetSingleAsync(x => x.UserId == userId && x.Target == target && x.Purpose == purpose, cancellationToken, isTracking);
}
