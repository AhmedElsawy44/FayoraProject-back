using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Services.Chatbot
{
    public class AiChatService : IAiChatService
    {
        public Task<string> ExtractSearchParametersAsync(string userMessage, List<ChatMessageDto> chatHistory, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> HumanizeRecommendationAsync(TravelOptionDto selectedOption, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
