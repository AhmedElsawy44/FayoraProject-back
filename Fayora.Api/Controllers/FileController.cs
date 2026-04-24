using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Commands.UploadFiles;
using Fayora.Contracts.AuthModule.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/files")]
public class FileController(ISender sender) : ApiController
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles(
        [FromForm] UploadFileRequest request,
        CancellationToken cancellationToken)
    {
        var (contextOk, context) = EnumParser.TryParseEnum<UploadContext>(request.Context);

        if (!contextOk) return BadRequest("Invalid context value.");

        var command = new UploadFilesCommand(request.Files, context);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }
}
