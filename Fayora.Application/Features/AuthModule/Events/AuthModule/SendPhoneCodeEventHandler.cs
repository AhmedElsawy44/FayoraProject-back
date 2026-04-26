using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Services.AuthModule.IMessageGenerator;

namespace Fayora.Application.Features.AuthModule.Events.AuthModule;

public class SendPhoneCodeEventHandler(
    IMessageGenerator messageGenerator,
    IMessageService messageService,
    ILogger<SendPhoneCodeEventHandler> logger)
    : INotificationHandler<PhoneCodeRequestedEvent>
{
    public async Task Handle(PhoneCodeRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var messagePurpose = MapToMessagePurpose(notification.Purpose);

            string message = notification.DeliveryMethod switch
            {
                CodeDeliveryMethod.Sms => messageGenerator.CreateSmsMessage(messagePurpose, notification.Code),
                CodeDeliveryMethod.WhatsApp => messageGenerator.CreateWhatsAppMessage(messagePurpose, notification.Code),
                _ => throw new NotSupportedException()
            };

            await messageService.SendMessageAsync(
                notification.PhoneNumber,
                message,
                notification.DeliveryMethod);

            logger.LogInformation("Successfully sent {Method} code to {Phone}",
                notification.DeliveryMethod, notification.PhoneNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send code to {Phone} via {Method}",
                notification.PhoneNumber, notification.DeliveryMethod);
        }
    }

    private static MessagePurpose MapToMessagePurpose(CodePurpose purpose)
    {
        return purpose switch
        {
            CodePurpose.VerifyAccount => MessagePurpose.Registration,
            CodePurpose.ResetPassword => MessagePurpose.ResetPassword,
            CodePurpose.AccountDeletion => MessagePurpose.AccountDeletion,
            CodePurpose.ReactivateAccount => MessagePurpose.ReactivateAccount,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), $"Unexpected OTP purpose: {purpose}")
        };
    }
}