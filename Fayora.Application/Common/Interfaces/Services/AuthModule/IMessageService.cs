using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IMessageService
{
    Task<bool> SendMessageAsync(string to, string message, CodeDeliveryMethod method);
}
