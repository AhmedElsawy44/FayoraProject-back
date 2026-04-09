using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.ChatModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.ChatModule;
using Fayora.Domain.Enums.ChatModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.ChatModule.IChatRepository;

namespace Fayora.Application.Features.ChatModule.Commands.SendMessage;

public class SendMessageCommandHandler(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : IRequestHandler<SendMessageCommand, Result<SendMessageResult>>
{
    public async Task<Result<SendMessageResult>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var messageType = Enum.Parse<MessageType>(request.MessageType, ignoreCase: true);
        var scopeType = Enum.Parse<ChatScopeType>(request.ScopeType, ignoreCase: true);

        var context = clientContextProvider.GetContext();

        var senderId = context.UserId;
        var name = context.UserName;
        var avatarUrl = context.UserAvatarUrl;

        Chat? chat = null;

        if (request.ChatId is not null)
        {
            chat = await chatRepository.GetChatByIdAsync(request.ChatId.Value, false, cancellationToken);

            if (chat is null
                ||
                    !((chat.FirstUserId == senderId && chat.SecondUserId == request.ReceiverId)
                        ||
                            (chat.FirstUserId == request.ReceiverId && chat.SecondUserId == senderId)
                    )
                )

            {
                return ChatErrors.ChatNotFound;
            }
        }
        else
        {
            chat = await chatRepository.GetByParticipantsAndScopeAsync(
                senderId,
                request.ReceiverId,
                scopeType,
                request.ScopeId,
                cancellationToken);

            if (chat is null)
            {
                var chatResult = Chat.Create(
                    senderId,
                    request.ReceiverId,
                    scopeType,
                    request.ScopeId);

                if (chatResult.IsError) return chatResult.Errors;

                chat = chatResult.Value;
                chatRepository.AddChat(chat);
            }
        }

        var message = Message.Create(
            chat.Id,
            senderId,
            request.Content,
            messageType);

        if (message.IsError) return message.Errors;

        messageRepository.AddMessage(message.Value);

        chat.UpdateLastMessageAt();

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new SendMessageResult(
            message.Value.Id,
            chat.Id,
            senderId,
            name,
            avatarUrl,
            message.Value.CreatedAt
        );
    }
}
