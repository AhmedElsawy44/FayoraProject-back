using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Services.AuthModule.IMessageGenerator;

namespace Fayora.Application.Features.AuthModule.Events;

public class SendPhoneCodeEventHandler(
    ISmsService smsService,
    IWhatsAppService whatsAppService,
    IMessageGenerator messageGenerator,
    ILogger<SendPhoneCodeEventHandler> logger)
    : INotificationHandler<PhoneCodeRequestedEvent>
{
    public async Task Handle(PhoneCodeRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var messagePurpose = MapToMessagePurpose(notification.Purpose);

            switch (notification.DeliveryMethod)
            {
                case CodeDeliveryMethod.Sms:
                    var smsMessage = messageGenerator.CreateSmsMessage(messagePurpose, notification.Code);
                    await smsService.SendSmsAsync(notification.PhoneNumber, smsMessage);
                    logger.LogInformation("OTP SMS sent successfully to {Phone}", notification.PhoneNumber);
                    break;

                case CodeDeliveryMethod.WhatsApp:
                    var whatsappMessage = messageGenerator.CreateWhatsAppMessage(messagePurpose, notification.Code);

                    try
                    {
                        await whatsAppService.SendWhatsAppMessageAsync(notification.PhoneNumber, whatsappMessage);
                        logger.LogInformation("OTP WhatsApp sent successfully to {Phone}", notification.PhoneNumber);
                    }
                    catch (Exception ex)
                    {
                        // خطة الطوارئ: إذا فشل الواتساب، نرسل SMS
                        logger.LogWarning(ex, "WhatsApp failed for {Phone}. Falling back to SMS.", notification.PhoneNumber);
                        var fallbackSms = messageGenerator.CreateSmsMessage(messagePurpose, notification.Code);
                        await smsService.SendSmsAsync(notification.PhoneNumber, fallbackSms);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP Phone Message to {Phone} for purpose {Purpose}", notification.PhoneNumber, notification.Purpose);
        }
    }

    private static MessagelPurpose MapToMessagePurpose(CodePurpose purpose)
    {
        return purpose switch
        {
            CodePurpose.Registration => MessagelPurpose.Registration,
            CodePurpose.ResetPassword => MessagelPurpose.ResetPassword,
            CodePurpose.AccountDeletion => MessagelPurpose.AccountDeletion,
            CodePurpose.ReactivateAccount => MessagelPurpose.ReactivateAccount,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), $"Unexpected OTP purpose: {purpose}")
        };
    }
}