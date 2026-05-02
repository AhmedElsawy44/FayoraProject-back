using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;
using Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Contracts.BookingModule.CreatePackageBooking;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class BookingController(ISender sender) : ApiController
{
    [HttpPost("package/{packageId}/book")]
    public async Task<IActionResult> BookPackageAsync(
        [FromRoute] Guid packageId,
        [FromBody] CreatePackageBookingRequest request,
        CancellationToken cancellationToken
        )
    {
        var (paymentMethodOk, paymentMethod) = EnumParser.TryParseEnum<PaymentMethodType>(request.PaymentMethodType);

        if (paymentMethodOk is false)
        {
            return BadRequest($"Invalid payment method type: {request.PaymentMethodType}");
        }

        var command = new CreatePackageBookingCommand
        (
            packageId,
            request.BookingDate,
            request.Adults,
            request.Children,
            paymentMethod
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> WebhookEndpoint(CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(HttpContext.Request.Body);
        var jsonPayload = await streamReader.ReadToEndAsync();
        var signature = Request.Headers["HMAC-Signature"].ToString();

        var command = new ProcessPaymentWebhookCommand(jsonPayload, signature);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            if (result.Errors.Contains(BookingErrors.InvalidPaymentWebhook))
            {
                return BadRequest("Invalid Signature or Payload");
            }

            return Ok();
        }

        return Ok();
    }
}
