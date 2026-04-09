using AutoMapper;
using Fayora.Application.Features.ChatModule.Queries.GetChats;
using Fayora.Application.Features.ChatModule.Queries.GetMessages;
using Fayora.Contracts.ChatModule.GetChats;
using Fayora.Contracts.ChatModule.GetMessages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class ChatController(ISender sender, IMapper mapper) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetChats(
        [FromQuery] GetChatsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetChatsQuery(request.Limit, request.Cursor);

        var result = await sender.Send(query, cancellationToken);

        return Ok(mapper.Map<GetChatsResponse>(result));
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(
        [FromRoute] Guid chatId,
        [FromQuery] GetMessagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMessagesQuery(chatId, request.Limit, request.Cursor);

        var result = await sender.Send(query, cancellationToken);

        return Ok(mapper.Map<GetMessagesResponse>(result));
    }

}
