namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IWhatsAppService
{
    Task SendWhatsAppMessageAsync(string phoneNumber, string message);
}