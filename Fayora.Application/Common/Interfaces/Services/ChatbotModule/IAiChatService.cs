using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

namespace Fayora.Application.Common.Interfaces.Services.ChatbotModule;

public interface IAiChatService
{
    Task<string> ExtractSearchParametersAsync(
        string userMessage,
        List<ChatMessageDto> chatHistory,
        CancellationToken cancellationToken);

    Task<string> HumanizeRecommendationAsync(
        TravelOptionDto selectedOption,
        CancellationToken cancellationToken);
}
