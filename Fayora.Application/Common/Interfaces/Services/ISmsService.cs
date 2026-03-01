namespace Fayora.Application.Common.Interfaces.Services;

public interface ISmsService
{
    Task SendSMSAsync(string phoneNumber, string text);
}
