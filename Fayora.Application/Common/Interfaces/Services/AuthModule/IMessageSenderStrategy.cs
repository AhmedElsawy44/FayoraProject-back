using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IMessageSenderStrategy
{
    CodeDeliveryMethod Method { get; }
    Task<bool> SendAsync(string to, string message);
}
