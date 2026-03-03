using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCode(Guid userId, string target, OtpPurpose purpose, CancellationToken cancellationToken, bool isTracking = false);
}
