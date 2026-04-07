using Fayora.Domain.Entities.ChatModule;

namespace Fayora.Application.Common.Interfaces.Presistances.ChatModule;

public interface IMessageRepository
{
    void AddMessage(Message value);
}
