using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Events;

public class PasswordResetedEventHandler(
    ILogger<PasswordResetedEventHandler> logger,
    IEmailService emailService) : INotificationHandler<PasswordResetedEvent>
{
    public async Task Handle(PasswordResetedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset successfully for User: {UserId}, Email: {Email}",
            notification.UserId, notification.Email);

        var subject = "Security Alert: Password Changed";
        var message = GeneratePasswordResetMessage(notification.Email);

        try
        {
            await emailService.SendEmailAsync(
                notification.Email,
                subject,
                message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset confirmation email to {Email}", notification.Email);
        }
    }

    private string GeneratePasswordResetMessage(string email)
    {
        return $"""
                Hi {email},
                
                This is a confirmation that the password for your Fayora account has been successfully changed.
                
                If you did not make this change, please contact our support team immediately to secure your account.
                
                Best regards,
                Fayora Team
                """;
    }
}