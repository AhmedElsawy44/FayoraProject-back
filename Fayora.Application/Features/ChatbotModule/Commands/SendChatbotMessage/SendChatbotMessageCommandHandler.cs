using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

public class SendChatbotMessageCommandHandler(
    IChatbotInteractionService chatbotInteractionService)
    : ICommandHandler<SendChatbotMessageCommand, Result<ChatbotMessageResult>>
{
    public async Task<Result<ChatbotMessageResult>> Handle(
        SendChatbotMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await chatbotInteractionService.ProcessMessageAsync(
            request.DeviceId,
            request.Content,
            request.SessionId,
            request.UserId,
            cancellationToken);

        return result;
    }
}
