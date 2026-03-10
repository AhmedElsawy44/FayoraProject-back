namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
