using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.ChatModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;

public class MarkChatAsReadCommandHandler(
    IChatRepository chatRepository,
    IMessageRepository messageRepository, // 👈 حقننا الـ Message Repo
    IClientContextProvider clientContextProvider
    ) : IRequestHandler<MarkChatAsReadCommand, Result<MarkChatAsReadResult>>
{
    public async Task<Result<MarkChatAsReadResult>> Handle(MarkChatAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var chat = await chatRepository.GetChatByIdAsync(request.ChatId, true, cancellationToken);

        if (chat is null || (chat.FirstUserId != userId && chat.SecondUserId != userId))
            return ChatErrors.ChatNotFound;

        var receiverId = chat.FirstUserId == userId ? chat.SecondUserId : chat.FirstUserId;

        await messageRepository.MarkMessagesAsReadAsync(request.ChatId, userId, cancellationToken);

        return new MarkChatAsReadResult(
            receiverId,
            chat.Id,
            DateTimeOffset.UtcNow
        );
    }
}