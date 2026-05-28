using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using Fayora.Application.Features.ChatbotModule.Queries.GetChatbotHistory;
using Fayora.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Fayora.Api.Hubs;

public class ChatbotHub(ISender sender) : Hub<IChatbotClient>
{
    //public async Task SendMessage(string deviceId, string content, Guid? sessionId)
    //{
    //    if (string.IsNullOrWhiteSpace(deviceId))
    //    {
    //        await Clients.Caller.ReceiveError(new[] { Error.Validation("DeviceId", "Device ID is required.") });
    //        return;
    //    }

    //    var command = new SendChatbotMessageCommand(
    //        deviceId,
    //        content,
    //        sessionId);

    //    var result = await sender.Send(command);

    //    if (result.IsSuccess)
    //    {
    //        object? responseObj = null;
    //        try
    //        {
    //            responseObj = JsonSerializer.Deserialize<object>(result.Value.ResponseJson);
    //        }
    //        catch
    //        {
    //            // Fallback to text if the response cannot be parsed as JSON
    //            responseObj = new { text = result.Value.ResponseJson };
    //        }

    //        await Clients.Caller.ReceiveChatbotMessage(new
    //        {
    //            SessionId = result.Value.SessionId,
    //            Response = responseObj
    //        });
    //    }
    //    else
    //    {
    //        await Clients.Caller.ReceiveError(result.Errors);
    //    }
    //}

    public async Task GetHistory(string deviceId, Guid? sessionId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            await Clients.Caller.ReceiveError(new[] { Error.Validation("DeviceId", "Device ID is required.") });
            return;
        }

        var query = new GetChatbotHistoryQuery(deviceId, sessionId);

        var result = await sender.Send(query);

        if (result.IsSuccess)
        {
            await Clients.Caller.ReceiveChatbotHistory(result.Value);
        }
        else
        {
            await Clients.Caller.ReceiveError(result.Errors);
        }
    }
}
