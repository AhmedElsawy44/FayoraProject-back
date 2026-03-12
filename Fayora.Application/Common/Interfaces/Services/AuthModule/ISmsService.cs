namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string text);
}
