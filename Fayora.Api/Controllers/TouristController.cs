using AutoMapper;
using Fayora.Application.Features.Tourist.Commands.CreateTouristProfile;
using Fayora.Application.Features.Tourist.Queries.GetInterests;
using Fayora.Contracts.Tourist;
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

    [HttpPost("Profile")]
    public async Task<IActionResult> CreateTouristProfileAsync(CreateTouristProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(mapper.Map<CreateTouristProfileResult>(result.Value))
            : BadRequest(result.Errors);
    }
}