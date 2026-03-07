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
    IWhatsAppService whatsAppService,
    IMessageGenerator messageGenerator,
    ILogger<SendOtpEventHandler> logger)
    : INotificationHandler<CodeRequestedEvent>
{
    public async Task Handle(CodeRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var messagePurpose = MapToMessagePurpose(notification.Purpose);

            switch (notification.DeliveryMethod)
            {
                case CodeDeliveryMethod.Email:
                    var (subject, body) = messageGenerator.CreateEmailMessage(messagePurpose, notification.Code);
                    await emailService.SendEmailAsync(notification.Target, subject, body);
                    logger.LogInformation("OTP email sent successfully to {Target}", notification.Target);
                    break;

                case CodeDeliveryMethod.Sms:
                    var message = messageGenerator.CreateSmsMessage(messagePurpose, notification.Code);
                    await smsService.SendSmsAsync(notification.Target, message);
                    logger.LogInformation("OTP SMS sent successfully to {Target}", notification.Target);
                    break;

                case CodeDeliveryMethod.WhatsApp:
                    var whatsappMessage = messageGenerator.CreateWhatsAppMessage(messagePurpose, notification.Code);
                    await whatsAppService.SendWhatsAppMessageAsync(notification.Target, whatsappMessage);
                    logger.LogInformation("OTP WhatsApp message sent successfully to {Target}", notification.Target);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP to {Target} for purpose {Purpose}", notification.Target, notification.Purpose);
        }
    }

    private static MessagelPurpose MapToMessagePurpose(CodePurpose purpose)
    {
        return purpose switch
        {
            CodePurpose.Registration => MessagelPurpose.Registration,
            CodePurpose.ResetPassword => MessagelPurpose.ResetPassword,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), $"Unexpected OTP purpose: {purpose}")
        };
    }
}