using Fayora.Contracts.ChatbotModule;

namespace Fayora.Api.Hubs;

public interface IChatbotClient
{
    Task ReceiveChatbotMessage(object payload);
    Task ReceiveChatbotHistory(List<ChatbotMessageResponse> history);
    Task ReceiveError(object errors);
}
