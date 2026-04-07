using Fayora.Application.Features.ChatModule.SendMessage;
using Fayora.Application.Features.ChatModule.UpdateMessage;
using Fayora.Contracts.ChatModule.SendMessage;
using Fayora.Contracts.ChatModule.UpdateMessage;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fayora.Api.Hubs;

public class ChatHub(ISender sender) : Hub
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
            await Clients.User(request.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", new
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
                    SentAt = DateTimeOffset.UtcNow
                });
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveError", result.Errors);
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

            await Clients.User(result.Value.ReceiverId.ToString()).SendAsync("MessageUpdated", responsePayload);

            await Clients.Caller.SendAsync("MessageUpdateSuccess", responsePayload);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveError", result.Errors);
        }
    }
}
