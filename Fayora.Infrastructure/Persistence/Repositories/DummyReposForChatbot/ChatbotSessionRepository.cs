using Fayora.Application.Common.Interfaces.Persistences.ChatbotModule;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.DummyReposForChatbot
{
    public class ChatbotSessionRepository : IChatbotSessionRepository
    {
        public Task<List<ChatMessageDto>> GetSessionHistoryAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
