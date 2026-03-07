namespace Fayora.Application.Common.Interfaces.Services;

public interface IWhatsAppService
{
    Task SendWhatsAppMessageAsync(string phoneNumber, string message);
}