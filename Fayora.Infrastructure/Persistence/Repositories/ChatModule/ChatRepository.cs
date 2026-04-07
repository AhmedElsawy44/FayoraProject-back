using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Domain.Entities.ChatModule;
using Fayora.Domain.Enums.ChatModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.ChatModule;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    public void AddChat(Chat chat)
    {
        context.Chats.Add(chat);
    }

    public Task<Chat?> GetChatByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Chats
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
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
                // 1. لازم نتأكد إن اليوزرز هما أطراف الشات (سواء مين بدأ الأول)
                ((c.FirstUserId == senderId && c.SecondUserId == receiverId) ||
                 (c.FirstUserId == receiverId && c.SecondUserId == senderId))

                // 2. ولازم الشات يكون لنفس الـ Scope (نفس الرحلة أو الوحدة)
                && c.ScopeType == scopeType
                && c.ScopeId == scopeId,

                cancellationToken);
    }
}