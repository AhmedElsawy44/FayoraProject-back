using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Events;
using Fayora.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Services.IMessageGenerator;

namespace Fayora.Application.Features.Auth.Events;

public class SendOtpEventHandler(
    ISmsService smsService,
    IEmailService emailService,
    IMessageGenerator messageGenerator,
    ILogger<SendOtpEventHandler> logger)
    : INotificationHandler<OtpRequestedEvent>
{
    public async Task Handle(OtpRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var messagePurpose = MapToMessagePurpose(notification.Purpose);

            if (notification.Target.Contains('@'))
            {
                var (subject, body) = messageGenerator.CreateEmailMessage(messagePurpose, notification.Code);

                await emailService.SendEmailAsync(notification.Target, subject, body);

                logger.LogInformation("OTP email sent successfully to {Target}", notification.Target);
            }
            else
            {
                var message = messageGenerator.CreateSmsMessage(messagePurpose, notification.Code);

                await smsService.SendSmsAsync(notification.Target, message);

                logger.LogInformation("OTP SMS sent successfully to {Target}", notification.Target);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP to {Target} for purpose {Purpose}", notification.Target, notification.Purpose);
        }
    }

    private static MessagelPurpose MapToMessagePurpose(OtpPurpose purpose)
    {
        return purpose switch
        {
            OtpPurpose.Registration => MessagelPurpose.Registration,
            OtpPurpose.ResetPassword => MessagelPurpose.ResetPassword,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), $"Unexpected OTP purpose: {purpose}")
        };
    }
}