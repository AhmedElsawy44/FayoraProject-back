using Fayora.Application.Features.ExploreModule.Queries.GetExploreItems;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExploreController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetExploreItems(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetExploreItemsQuery(pageNumber, pageSize, search);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            Ok,
            Problem
        );
    }
}
