using System;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

public class SendChatbotMessageCommandHandler(
    IChatbotInteractionService chatbotInteractionService,
    IClientContextProvider clientContextProvider)
    : ICommandHandler<SendChatbotMessageCommand, Result<ChatbotMessageResult>>
{
    public async Task<Result<ChatbotMessageResult>> Handle(
        SendChatbotMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userIdResult = clientContextProvider.GetContext().UserId;
        Guid? userId = userIdResult == Guid.Empty ? null : userIdResult;

        var result = await chatbotInteractionService.ProcessMessageAsync(
            request.DeviceId,
            request.Content,
            request.SessionId,
            userId,
            cancellationToken);

        return Result<ChatbotMessageResult>.CreateSuccess(result);
    }
}
