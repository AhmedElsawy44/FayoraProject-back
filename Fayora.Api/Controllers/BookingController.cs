using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.BookingModule.Commands.CreateAccommodationBooking;
using Fayora.Application.Features.BookingModule.Commands.CreateGuideBooking;
using Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;
using Fayora.Application.Features.BookingModule.Commands.GenerateBookingQr;
using Fayora.Application.Features.BookingModule.Commands.GetMyBookings;
using Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook;
using Fayora.Application.Features.BookingModule.Commands.ScanBookingQr;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Contracts.BookingModule.CreateGuideBooking;
using Fayora.Contracts.BookingModule.CreatePackageBooking;
using Fayora.Contracts.BookingModule.CreateUnitBooking;
using Fayora.Contracts.BookingModule.ScanBookingQr;
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


    [HttpPost("unit/{unitId:guid}/book")]
    public async Task<IActionResult> BookUnitAsync(
    [FromRoute] Guid unitId,
    [FromBody] CreateAccommodationBookingRequest request,
    CancellationToken cancellationToken)
    {
        var (paymentMethodOk, paymentMethod) = EnumParser.TryParseEnum<PaymentMethodType>(request.PaymentMethodType);

        if (!paymentMethodOk)
            return BadRequest($"Invalid payment method type: {request.PaymentMethodType}");

        var command = new CreateAccommodationBookingCommand(
            unitId,
            request.StartDate,
            request.EndDate,
            request.Adults,
            request.Children,
            paymentMethod
        );

        var result = await sender.Send(command, cancellationToken);
        return result.Match(Ok, Problem);
    }


    [HttpPost("guide/{guideId:guid}/book")]
    public async Task<IActionResult> BookGuideAsync(
    [FromRoute] Guid guideId,
    [FromBody] CreateGuideBookingRequest request,
    CancellationToken cancellationToken)
    {
        var (paymentMethodOk, paymentMethod) =
            EnumParser.TryParseEnum<PaymentMethodType>(request.PaymentMethodType);

        if (!paymentMethodOk)
            return BadRequest($"Invalid payment method type: {request.PaymentMethodType}");

        var command = new CreateGuideBookingCommand(
            guideId,
            request.BookingDate,
            request.StartTime,
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


    [HttpGet("{bookingId:guid}/generateQr")]
    public async Task<IActionResult> GenerateQr(
    Guid bookingId,
    CancellationToken cancellationToken)
    {
        var command = new GenerateBookingQrCommand(bookingId);
        var result = await sender.Send(command, cancellationToken);
        return result.Match(Ok, Problem);
    }

    [HttpPost("scanQr")]
    public async Task<IActionResult> ScanQr(
        [FromBody] ScanBookingQrRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ScanBookingQrCommand(request.Token);
        var result = await sender.Send(command, cancellationToken);
        return result.Match(Ok, Problem);
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetMyBookingsQuery(page, pageSize);

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }
}
