using Fayora.Application.Common.Interfaces.Persistences.ChatModule;
using Fayora.Domain.Entities.ChatModule;
using Fayora.Domain.Enums.ChatModule;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Persistences.ChatModule.IChatRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.ChatModule;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    public void AddChat(Chat chat)
    {
        context.Chats.Add(chat);
    }

    public Task<Chat?> GetChatByIdAsync(Guid id, bool isReadOnly, CancellationToken cancellationToken)
    {
        if (isReadOnly)
            return context.Chats.AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return context.Chats.FindAsync(new object[] { id }, cancellationToken).AsTask();
    }

    public Task<Chat?> GetByParticipantsAndScopeAsync(
        Guid senderId,
        Guid receiverId,
        ChatScopeType scopeType,
        Guid scopeId,
        CancellationToken cancellationToken)
    {
        return context.Chats
            .FirstOrDefaultAsync(c =>
                ((c.FirstUserId == senderId && c.SecondUserId == receiverId) ||
                 (c.FirstUserId == receiverId && c.SecondUserId == senderId))
                && c.ScopeType == scopeType
                && c.ScopeId == scopeId,

                cancellationToken);
    }

    public async Task<List<ChatItemDto>> GetUserChatsPagedAsync(Guid userId, int limit, DateTimeOffset? cursor, CancellationToken cancellationToken = default)
    {
        var query = context.Chats
            .AsNoTracking()
            .Where(c => c.FirstUserId == userId || c.SecondUserId == userId);

        if (cursor.HasValue)
        {
            query = query.Where(c => c.UpdatedAt < cursor.Value);
        }

        var chats = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Take(limit)
            .Select(c => new ChatItemDto
            (
                c.Id,
                c.FirstUserId == userId ? c.SecondUserId : c.FirstUserId,
                string.Empty,
                null,

                c.Messages.OrderByDescending(m => m.CreatedAt).Select(m => m.Content).FirstOrDefault(),
                c.Messages.OrderByDescending(m => m.CreatedAt).Select(m => m.CreatedAt).FirstOrDefault(),

                c.Messages.Count(m => m.SenderId != userId && !m.ReadAt.HasValue)
            ))
            .ToListAsync(cancellationToken);

        return chats;
    }
}