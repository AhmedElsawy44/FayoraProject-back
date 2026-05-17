using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AdminModule.Commands.CreateLocation;
using Fayora.Application.Features.AdminModule.Commands.DeleteLocation;
using Fayora.Application.Features.AdminModule.Commands.VerifyContent;
using Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;
using Fayora.Application.Features.AdminModule.Queries.GetDetailedPackage;
using Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;
using Fayora.Application.Features.AdminModule.Queries.GetInventoryQueue;
using Fayora.Application.Features.AdminModule.Queries.GetInventoryStats;
using Fayora.Application.Features.AdminModule.Queries.GetTourCompanyVerificationDetails;
using Fayora.Application.Features.AdminModule.Queries.GetTourGuidesStat;
using Fayora.Application.Features.AdminModule.Queries.GetTourGuideVerificationDetails;
using Fayora.Application.Features.AdminModule.Queries.GetTravelAgenciesStats;
using Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;
using Fayora.Contracts.AdminModule.CreateLocation;
using Fayora.Contracts.AdminModule.GetVerificationQueue;
using Fayora.Contracts.AdminModule.VerifyContent;
using Fayora.Domain.Enums.SharedModule;
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
            request.Rating,
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

    [HttpDelete("locations/{id}")]
    public async Task<IActionResult> DeleteLocation(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(new { message = value }),
            Problem);
    }

    [HttpGet("financial-stats")]
    public async Task<IActionResult> GetFinancialStats(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var finalStartDate = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var finalEndDate = endDate ?? DateTime.UtcNow;

        var query = new GetFinancialStatsQuery(finalStartDate, finalEndDate);

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("calendar-packages")]
    public async Task<IActionResult> GetCalendarPackages(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var query = new GetCalendarBookingsQuery(year, month);

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("tour-guides-stats")]
    public async Task<IActionResult> GetTourGuidesStats(CancellationToken cancellationToken)
    {
        var query = new GetTourGuidesStatsQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("travel-agencies-stats")]
    public async Task<IActionResult> GetTravelAgenciesStats(CancellationToken cancellationToken)
    {
        var query = new GetTravelAgenciesStatsQuery();
        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("inventory-stats")]
    public async Task<IActionResult> GetInventoryStats(CancellationToken cancellationToken)
    {
        var query = new GetInventoryStatsQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("verification-queue")]
    public async Task<IActionResult> GetTravelAgenciesStats(
        [FromQuery] GetVerificationQueueRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetVerificationQueueQuery(
            request.Type,
            request.PageNumber,
            request.PageSize
        );

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }


    [HttpGet("verification-queue/guide/{id:guid}")]
    public async Task<IActionResult> GetTourGuideVerificationDetails(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTourGuideVerificationDetailsQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpGet("verification-queue/company/{id:guid}")]
    public async Task<IActionResult> GetTourCompanyVerificationDetails(
    Guid id,
    CancellationToken cancellationToken)
    {
        var query = new GetTourCompanyVerificationDetailsQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}

