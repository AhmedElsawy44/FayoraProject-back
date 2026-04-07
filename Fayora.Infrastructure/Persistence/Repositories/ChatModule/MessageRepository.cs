using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Domain.Entities.ChatModule;

namespace Fayora.Infrastructure.Persistence.Repositories.ChatModule;

public class MessageRepository(ApplicationDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }
}
