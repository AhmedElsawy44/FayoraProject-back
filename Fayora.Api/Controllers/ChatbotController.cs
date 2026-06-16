using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using Fayora.Contracts.ChatbotModule;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class ChatbotController(ISender sender) : ApiController
{
    [HttpPost("message")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendChatbotMessageRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return BadRequest("Device ID is required in X-Device-Id header.");
        }

        var command = new SendChatbotMessageCommand(
            deviceId,
            request.Content,
            request.SessionId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            success =>
            {
                object? responseObj = null;
                try
                {
                    responseObj = JsonSerializer.Deserialize<object>(success.ResponseJson);
                }
                catch
                {
                    responseObj = new { text = success.ResponseJson };
                }

                return Ok(new
                {
                    SessionId = success.SessionId,
                    Response = responseObj
                });
            },
            Problem
        );
    }
}
