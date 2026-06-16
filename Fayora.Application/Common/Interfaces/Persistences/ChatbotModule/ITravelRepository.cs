using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

namespace Fayora.Application.Common.Interfaces.Persistences.ChatbotModule;
public interface ITravelRepository
{
    Task<List<TravelOptionDto>> SearchTravelOptionsAsync(
        TravelSearchParametersDto searchParameters,
        CancellationToken cancellationToken);
}
