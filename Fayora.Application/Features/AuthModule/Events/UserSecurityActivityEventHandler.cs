using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Events.IdentityModule;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Services.AuthModule.IMessageGenerator;

namespace Fayora.Application.Features.AuthModule.Events;

public class UserSecurityActivityEventHandler(
    ILogger<UserSecurityActivityEventHandler> logger,
    IEmailService emailService,
    IMessageGenerator messageGenerator)
    : INotificationHandler<UserSecurityActivityDomainEvent>
{
    public async Task Handle(UserSecurityActivityDomainEvent notification, CancellationToken cancellationToken)
    {
        bool isEmail = notification.Target.Contains('@');

        if (!isEmail)
        {
            return;
        }

        string email = notification.Target;

        switch (notification.ActivityType)
        {
            case SecurityActivityType.PasswordReset:
                await SendSecurityAlertAsync(email, MessagePurpose.ResetPassword, notification.UserId);
                break;

            case SecurityActivityType.EmailChanged:
                await SendSecurityAlertAsync(email, MessagePurpose.ChangeEmail, notification.UserId);
                break;

            case SecurityActivityType.AccountDeleted:
                await SendSecurityAlertAsync(email, MessagePurpose.AccountDeletion, notification.UserId);
                break;
            case SecurityActivityType.EmailVerified:
                await SendSecurityAlertAsync(email, MessagePurpose.EmailVerified, notification.UserId);
                break;
        }
    }

    private async Task SendSecurityAlertAsync(string email, MessagePurpose purpose, Guid userId)
    {
        var (subject, message) = messageGenerator.CreateEmailMessage(purpose);

        try
        {
            await emailService.SendEmailAsync(email, subject, message);
            logger.LogInformation("Security alert ({Purpose}) sent successfully to User: {UserId}, Email: {Email}",
                purpose.ToString(), userId, email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send security alert ({Purpose}) to {Email}", purpose.ToString(), email);
        }
    }
}