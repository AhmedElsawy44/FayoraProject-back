using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Infrastructure.Services.AuthModule;

public class MessageService(IEnumerable<IMessageSenderStrategy> strategies) : IMessageService
{
    private readonly IReadOnlyDictionary<CodeDeliveryMethod, IMessageSenderStrategy> _strategies =
        strategies.ToDictionary(strategy => strategy.Method);

    public Task<bool> SendMessageAsync(string to, string message, CodeDeliveryMethod method)
    {
        if (!_strategies.TryGetValue(method, out var strategy))
        {
            throw new NotSupportedException($"No message sender strategy registered for {method}.");
        }

        return strategy.SendAsync(to, message);
    }
}
