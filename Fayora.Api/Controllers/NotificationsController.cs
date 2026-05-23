using Fayora.Application.Features.TouristModule.Commands.RegisterDeviceToken;
using Fayora.Application.Features.NotificationModule.Queries.GetNotifications;
using Fayora.Application.Features.NotificationModule.Queries.GetUnreadCount;
using Fayora.Application.Features.NotificationModule.Commands.MarkAsRead;
using Fayora.Application.Features.NotificationModule.Commands.MarkAllAsRead;
using Fayora.Contracts.AdminModule.Notifications;
using Fayora.Contracts.NotificationModule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(ISender sender) : ApiController
{
    [HttpPost("register-token")]
    public async Task<IActionResult> RegisterDeviceToken(
        [FromBody] RegisterDeviceTokenRequest request,
        CancellationToken ct)
    {
        var command = new RegisterDeviceTokenCommand(request.Token, request.DeviceType);
        var result = await sender.Send(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] GetNotificationsQueryRequest request,
        CancellationToken ct)
    {
        var query = new GetNotificationsQuery(request.PageNumber, request.PageSize);
        var result = await sender.Send(query, ct);
        return result.Match(Ok, Problem);
    }

    [Authorize]
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var query = new GetUnreadCountQuery();
        var result = await sender.Send(query, ct);
        return result.Match(Ok, Problem);
    }

    [Authorize]
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        var command = new MarkAsReadCommand(id);
        var result = await sender.Send(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [Authorize]
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var command = new MarkAllAsReadCommand();
        var result = await sender.Send(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }
}
