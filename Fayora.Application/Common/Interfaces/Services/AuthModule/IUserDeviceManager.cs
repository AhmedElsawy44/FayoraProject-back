using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IUserDeviceManager
{
    Task UpsertDeviceAsync(Guid userId, string deviceId, string fcmToken, Language deviceLanguage, CancellationToken cancellationToken = default);
}