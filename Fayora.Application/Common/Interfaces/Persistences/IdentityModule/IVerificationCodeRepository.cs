using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Persistences.IdentityModule;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, CodePurpose purpose, CancellationToken cancellationToken, bool isTracking = true);

}
