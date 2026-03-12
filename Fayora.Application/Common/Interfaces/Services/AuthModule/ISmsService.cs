namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string text);
}
