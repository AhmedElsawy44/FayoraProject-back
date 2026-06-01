using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Services.Chatbot
{
    public class RecommendationEngineService : IRecommendationEngineService
    {
        public Task<TravelOptionDto> SelectBestOptionAsync(List<TravelOptionDto> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
