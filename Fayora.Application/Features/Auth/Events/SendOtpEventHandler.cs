using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Events;
using Fayora.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Events;

public class SendOtpEventHandler(
    ISmsService smsService,
    IEmailService emailService,
    ILogger<SendOtpEventHandler> logger)
    : INotificationHandler<OtpRequestedDomainEvent>
{
    public async Task Handle(OtpRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var message = notification.Purpose switch
        {
            OtpPurpose.Registration => $"Welcome to Fayora! Your verification code is: {notification.Code}",
            OtpPurpose.ResetPassword => $"Your password reset code is: {notification.Code}. Don't share it!",
            OtpPurpose.Login => $"Your login code is: {notification.Code}",
            _ => $"Your Fayora code is: {notification.Code}"
        };

        try
        {
            if (notification.Target.Contains("@"))
            {
                await emailService.SendEmailAsync(notification.Target, notification.Purpose.ToString(), message);
                logger.LogInformation("OTP email sent successfully to {Target}", notification.Target);
            }
            else
            {
                await smsService.SendSMSAsync(notification.Target, message);
                logger.LogInformation("OTP SMS sent successfully to {Target}", notification.Target);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP to {Target} for purpose {Purpose}", notification.Target, notification.Purpose);
        }
    }
}
