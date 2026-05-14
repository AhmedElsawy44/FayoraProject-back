using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AdminModule.Commands.CreateLocation;
using Fayora.Application.Features.AdminModule.Commands.VerifyContent;
using Fayora.Application.Features.AdminModule.Queries.GetDetailedPackage;
using Fayora.Application.Features.AdminModule.Queries.GetInventoryQueue;
using Fayora.Contracts.AdminModule.CreateLocation;
using Fayora.Contracts.AdminModule.VerifyContent;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.ValueObjects;
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

    [HttpGet("package-details")]
    public async Task<IActionResult> GetPackageDetails([FromRoute] Guid PackageId)
    {
        var query = new GetDetailedPackageQuery(PackageId);

        var result = await sender.Send(query);

        return result.Match(Ok, Problem);
    }



    [HttpPost("locations")]
    public async Task<IActionResult> CreateLocation(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            (LocationCategory)request.Category,
            request.MainImageUrl,
            request.ImageUrls);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => CreatedAtAction(nameof(CreateLocation), new { locationId = value }),
            Problem);
    }
}
