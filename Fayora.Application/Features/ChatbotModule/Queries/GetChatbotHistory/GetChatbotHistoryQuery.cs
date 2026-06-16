using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.ChatbotModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatbotModule.Queries.GetChatbotHistory;

public record GetChatbotHistoryQuery(
    string DeviceId,
    Guid? SessionId = null
) : IQuery<Result<List<ChatbotMessageResponse>>>;
