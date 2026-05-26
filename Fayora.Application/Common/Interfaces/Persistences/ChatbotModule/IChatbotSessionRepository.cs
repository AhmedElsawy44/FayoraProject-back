using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

namespace Fayora.Application.Common.Interfaces.Persistences.ChatbotModule;

public interface IChatbotSessionRepository
{
    Task<List<ChatMessageDto>> GetSessionHistoryAsync(
        Guid sessionId,
        CancellationToken cancellationToken);
}
