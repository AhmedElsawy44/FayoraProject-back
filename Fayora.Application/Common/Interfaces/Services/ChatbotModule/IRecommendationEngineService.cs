using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

namespace Fayora.Application.Common.Interfaces.Services.ChatbotModule;

public interface IRecommendationEngineService
{
    Task<TravelOptionDto> SelectBestOptionAsync(
        List<TravelOptionDto> options,
        CancellationToken cancellationToken);
}
