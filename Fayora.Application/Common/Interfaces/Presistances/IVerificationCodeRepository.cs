using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, OtpPurpose purpose, CancellationToken cancellationToken, bool isTracking = true);
}
