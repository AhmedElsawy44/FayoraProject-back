using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Shared.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, CodePurpose purpose, CancellationToken cancellationToken, bool isTracking = true);
}
