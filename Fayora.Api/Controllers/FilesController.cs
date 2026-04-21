using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class FilesController : ApiController
{

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        return Ok();
    }
}