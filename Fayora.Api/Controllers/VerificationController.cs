using Fayora.Api.Requests;
using Fayora.Application.Features.Auth.Commands.SubmitVerificationRequest;
using Fayora.Domain.Enums.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Fayora.Api.Controllers
{

    [Route("api/verification")]
    public class VerificationController(ISender sender) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> SubmitVerificationRequest(
            [FromForm] SubmitVerificationRequestRequest request,
            CancellationToken ct)
        {
            var command = new SubmitVerificationRequestCommand(
                UserId: request.UserId,
                RequestType: request.RequestType,
                Documents: request.Documents
                    .Select(d => (d.DocumentType, d.File))
                    .ToList());

            var result = await sender.Send(command, ct);

            return result.Match(
                onValue: response => Ok(response),
                onError: Problem);
        }

    }

}
