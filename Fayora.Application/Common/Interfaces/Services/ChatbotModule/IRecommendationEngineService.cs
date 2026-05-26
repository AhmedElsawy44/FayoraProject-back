using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

namespace Fayora.Application.Common.Interfaces.Services.ChatbotModule;

/// <summary>
/// Service interface for the Recommendation Engine that filters and ranks
/// options to determine the best match for the user.
/// </summary>
public interface IRecommendationEngineService
{
    /// <summary>
    /// Evaluates a list of travel options and selects the absolute best match
    /// based on implicit scoring and matching logic.
    /// </summary>
    Task<TravelOptionDto> SelectBestOptionAsync(
        List<TravelOptionDto> options,
        CancellationToken cancellationToken);
}
