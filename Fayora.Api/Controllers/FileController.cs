using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Commands.UploadFiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/files")]
public class FileController(ISender sender) : ApiController
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles(
        [FromForm] List<IFormFile> files,
        [FromForm] string context,
        CancellationToken cancellationToken)
    {
        var (contextOk, contextResult) = EnumParser.TryParseEnum<UploadContext>(context);

        if (!contextOk) return BadRequest("Invalid context value.");

        var command = new UploadFilesCommand(files, contextResult);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}
