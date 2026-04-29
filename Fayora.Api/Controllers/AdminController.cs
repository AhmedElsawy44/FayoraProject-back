using Fayora.Application.Features.Admin.Commands.ApproveVerification;
using Fayora.Contracts.AdminModule.ApproveVerification;
using Fayora.Domain.Common.Results;
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
}
