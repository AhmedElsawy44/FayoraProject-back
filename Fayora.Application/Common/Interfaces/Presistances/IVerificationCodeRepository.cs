using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, CodePurpose purpose, CancellationToken cancellationToken, bool isTracking = true);
}
