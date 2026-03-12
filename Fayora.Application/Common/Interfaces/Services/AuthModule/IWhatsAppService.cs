namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IWhatsAppService
{
    Task SendWhatsAppMessageAsync(string phoneNumber, string message);
}