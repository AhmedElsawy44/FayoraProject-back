using Fayora.Application.Common.Interfaces.Persistences.ChatbotModule;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.DummyReposForChatbot
{
    public class TravelRepository : ITravelRepository
    {
        public Task<List<TravelOptionDto>> SearchTravelOptionsAsync(TravelSearchParametersDto searchParameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
