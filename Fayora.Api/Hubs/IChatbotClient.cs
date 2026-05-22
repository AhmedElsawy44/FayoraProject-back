using Fayora.Contracts.ChatbotModule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fayora.Api.Hubs;

public interface IChatbotClient
{
    Task ReceiveChatbotMessage(object payload);
    Task ReceiveChatbotHistory(List<ChatbotMessageResponse> history);
    Task ReceiveError(object errors);
}
