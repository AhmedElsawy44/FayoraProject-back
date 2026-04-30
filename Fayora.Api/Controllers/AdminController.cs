using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.Admin.Commands.VerifyContent;
using Fayora.Application.Features.Admin.Queries.GetInventoryQueueQuery;
using Fayora.Contracts.AdminModule.VerifyContent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;


[Route("api/[controller]")]
public class AdminController(ISender sender) : ApiController
{
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> VerifyContent(Guid id, [FromBody] VerifyContentRequest request, CancellationToken ct)
    {

        var command = new VerifyContentCommand(
            id,
            request.ItemType.ToString(),
            request.IsApproved,
            request.AdminNotes
        );


        var result = await sender.Send(command, ct);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("inventory-queue")]
    public async Task<IActionResult> GetInventoryQueue([FromQuery] TypeFilter? type, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var query = new GetInventoryQueueQuery(type, page, pageSize);
        var result = await sender.Send(query, ct);

        return Ok(result);
    }
}
