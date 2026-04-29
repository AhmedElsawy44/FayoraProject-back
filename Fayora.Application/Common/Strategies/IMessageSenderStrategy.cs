using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Strategies;

public interface IMessageSenderStrategy
{
    CodeDeliveryMethod Method { get; }
    Task<bool> SendAsync(string to, string message);
}
