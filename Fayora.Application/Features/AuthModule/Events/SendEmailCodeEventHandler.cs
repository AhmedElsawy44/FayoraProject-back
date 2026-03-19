using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Services.AuthModule.IMessageGenerator;

namespace Fayora.Application.Features.AuthModule.Events;

public class SendEmailCodeEventHandler(
    IEmailService emailService,
    IMessageGenerator messageGenerator,
    ILogger<SendEmailCodeEventHandler> logger)
    : INotificationHandler<EmailCodeRequestedEvent>
{
    public async Task Handle(EmailCodeRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var messagePurpose = MapToMessagePurpose(notification.Purpose);

            var (subject, body) = messageGenerator.CreateEmailMessage(messagePurpose, notification.Code);
            await emailService.SendEmailAsync(notification.Email, subject, body);

            logger.LogInformation("OTP email sent successfully to {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP Email to {Email} for purpose {Purpose}", notification.Email, notification.Purpose);
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
