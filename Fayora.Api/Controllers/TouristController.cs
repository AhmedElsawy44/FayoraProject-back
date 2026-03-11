using AutoMapper;
using Fayora.Application.Features.Tourist.Commands.CreateTouristProfile;
using Fayora.Application.Features.Tourist.Queries.GetInterests;
using Fayora.Contracts.Tourist;
using Fayora.Domain.Enums.TouristModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TouristController(ISender sender, IMapper mapper) : ApiController
{
    [HttpGet("Interests")]
    public async Task<IActionResult> GetInterestsAsync(CancellationToken cancellationToken)
    {
        var query = new GetInterestsQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(mapper.Map<InterestsResponse>(result));
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateTouristProfileAsync(
    [FromBody] CreateTouristRequest request,
    [FromHeader(Name = "X-Device-Id")] string deviceId,
    CancellationToken cancellationToken)
    {
        BudgetTier? budgetTier = null;
        TravelStyle? travelStyle = null;

        if (!string.IsNullOrWhiteSpace(request.BudgetTier))
        {
            if (!Enum.TryParse<BudgetTier>(request.BudgetTier, true, out var parsedBudget))
                return BadRequest("Invalid budget tier.");
            budgetTier = parsedBudget;
        }

        if (!string.IsNullOrWhiteSpace(request.TravelStyle))
        {
            if (!Enum.TryParse<TravelStyle>(request.TravelStyle, true, out var parsedStyle))
                return BadRequest("Invalid travel style.");
            travelStyle = parsedStyle;
        }

        var command = new CreateTouristProfileCommand(
            deviceId,
            budgetTier,
            travelStyle,
            request.InterestIds);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<CreateTouristResponse>(value)),
            Problem
        );
    }
}