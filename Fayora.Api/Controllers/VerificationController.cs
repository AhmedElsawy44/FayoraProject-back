using Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;
using Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;
using Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;
using Fayora.Contracts.VerificationModule;
using Fayora.Domain.Enums.SharedModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Fayora.Api.Controllers;


[Route("api/verification")]
public class VerificationController(ISender sender) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> SubmitVerificationRequest(
        [FromForm] SubmitVerificationRequestRequest request,
        CancellationToken cancellationToken)
    {
        var (success, requestType) = EnumParser.TryParseEnum<RequestType>(request.RequestType);
        if (!success)
            return BadRequest("Invalid request type.");

        var documents = new List<VerificationDocument>();
        foreach (var d in request.Documents)
        {
            var (docOk, docType) = EnumParser.TryParseEnum<DocumentType>(d.DocumentType);
            if (!docOk)
                return BadRequest("Invalid document type.");

            documents.Add(new VerificationDocument(docType, d.FileUrl));
        }

        var command = new SubmitVerificationRequestCommand(
            RequestType: requestType,
            Documents: documents);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetVerificationRequest(int id, CancellationToken cancellationToken)
    {
        var query = new GetVerificationRequestQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }


    [HttpPut("{id}/review")]
    public async Task<IActionResult> ReviewVerificationRequest(
        int id,
        [FromBody] ReviewVerificationRequestRequest request,
        CancellationToken cancellationToken)
    {
        var (success, newStatus) = EnumParser.TryParseEnum<RequestStatus>(request.NewStatus);
        if (!success)
            return BadRequest("Invalid request status.");

        var command = new ReviewVerificationRequestCommand(
            RequestId: id,
            NewStatus: newStatus,
            AdminComment: request.AdminComment);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

}
