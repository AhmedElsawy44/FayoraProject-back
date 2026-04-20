using Fayora.Api.Requests;
using MediatR;
using Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;
using Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;
using Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetVerificationRequest(int id, CancellationToken ct)
        {
            var query = new GetVerificationRequestQuery(id);
            var result = await sender.Send(query, ct);

            return result.Match(
                onValue: response => Ok(response),
                onError: Problem);
        }


        [HttpPut("{id}/review")]
        public async Task<IActionResult> ReviewVerificationRequest(int id, [FromBody] ReviewVerificationRequestRequest request, CancellationToken ct)
        {
            var command = new ReviewVerificationRequestCommand(
                RequestId: id,
                AdminId: request.AdminId,
                NewStatus: request.NewStatus,
                AdminComment: request.AdminComment);

            var result = await sender.Send(command, ct);

            return result.Match(
                onValue: response => Ok(response),
                onError: Problem);
        }

    }

}
