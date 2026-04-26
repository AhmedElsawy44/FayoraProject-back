using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Domain.Entities.ChatModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.ChatModule;

public class MessageRepository(ApplicationDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public async Task<List<Message>> GetChatMessagesPagedAsync(Guid chatId, int limit, DateTimeOffset? cursor, CancellationToken cancellationToken = default)
    {
        var query = context.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId);

        if (cursor.HasValue)
        {
            query = query.Where(m => m.CreatedAt < cursor.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return messages;
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await context.Messages.FindAsync(new object[] { messageId }, cancellationToken);
    }

    public Task MarkMessagesAsReadAsync(Guid chatId, Guid currentUserId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        return context.Messages
            .Where(m => m.ChatId == chatId && m.SenderId != currentUserId && m.ReadAt == null)

            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ReadAt, now)
                .SetProperty(m => m.UpdatedAt, now),
            cancellationToken);
    }
}
