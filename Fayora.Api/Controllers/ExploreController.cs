using Fayora.Application.Features.ExploreModule.Queries.GetExploreFeed;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExploreController(ISender sender) : ApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetExploreFeed(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null,
        CancellationToken ct = default)
    {
        var query = new GetExploreFeedQuery(pageNumber, pageSize, search, type);
        var result = await sender.Send(query, ct);
        return result.Match(Ok, Problem);
    }
}
