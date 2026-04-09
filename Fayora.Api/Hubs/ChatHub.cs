using Fayora.Application.Features.ChatModule.Commands.DeleteMessage;
using Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;
using Fayora.Application.Features.ChatModule.Commands.SendMessage;
using Fayora.Application.Features.ChatModule.Commands.UpdateMessage;
using Fayora.Contracts.ChatModule.SendMessage;
using Fayora.Contracts.ChatModule.UpdateMessage;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fayora.Api.Hubs;

public class ChatHub(ISender sender) : Hub<IChatClient>
{
    public async Task SendMessage(SendMessageRequest request)
    {
        var command = new SendMessageCommand(
            request.ReceiverId,
            request.Content,
            request.MessageType,
            request.ScopeType,
            request.ScopeId,
            request.ChatId
        );

        var result = await sender.Send(command);

        if (result.IsSuccess)
        {
            var responsePayload = new
            {
                MessageId = result.Value.MessageId,
                SenderId = result.Value.SenderId,
                SenderName = result.Value.SenderName,
                SenderAvatar = result.Value.SenderAvatarUrl,
                ChatId = result.Value.ChatId,
                ScopeType = request.ScopeType,
                ScopeId = request.ScopeId,
                Content = request.Content,
                MessageType = request.MessageType,
                SentAt = result.Value.SentAt
            };

            await Clients.User(request.ReceiverId.ToString()).ReceiveMessage(responsePayload);

            await Clients.Caller.MessageSentSuccess(responsePayload);
        }
        else
        {
            await Clients.Caller.ReceiveError(result.Errors);
        }
    }

    public async Task UpdateMessage(UpdateMessageRequest request)
    {
        var command = new UpdateMessageCommand(request.MessageId, request.NewContent);
        var result = await sender.Send(command);

        if (result.IsSuccess)
        {
            var responsePayload = new
            {
                MessageId = result.Value.MessageId,
                ChatId = result.Value.ChatId,
                NewContent = result.Value.Content,
                UpdatedAt = result.Value.UpdatedAt
            };

            await Clients.User(result.Value.ReceiverId.ToString()).MessageUpdated(responsePayload);

            await Clients.Caller.MessageUpdateSuccess(responsePayload);
        }
        else
        {
            await Clients.Caller.ReceiveError(result.Errors);
        }
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var command = new DeleteMessageCommand(messageId);

        var result = await sender.Send(command);

        if (result.IsSuccess)
        {
            var responsePayload = new
            {
                MessageId = result.Value.MessageId,
                ChatId = result.Value.ChatId,
                DeletedAt = result.Value.DeletedAt
            };

            await Clients.User(result.Value.ReceiverId.ToString()).MessageDeleted(responsePayload);

            await Clients.Caller.MessageDeleteSuccess(responsePayload);
        }
        else
        {
            await Clients.Caller.ReceiveError(result.Errors);
        }
    }

    public async Task MarkAsRead(Guid chatId)
    {
        var command = new MarkChatAsReadCommand(chatId);
        var result = await sender.Send(command);

        if (result.IsSuccess)
        {
            var responsePayload = new
            {
                ChatId = chatId,
                ReadAt = result.Value.ReadAt
            };

            await Clients.User(result.Value.ReceiverId.ToString()).MessagesSeen(responsePayload);
        }
    }
}