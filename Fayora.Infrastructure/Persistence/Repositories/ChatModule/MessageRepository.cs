using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Domain.Entities.ChatModule;

namespace Fayora.Infrastructure.Persistence.Repositories.ChatModule;

public class MessageRepository(ApplicationDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await context.Messages.FindAsync(new object[] { messageId }, cancellationToken);
    }
}
