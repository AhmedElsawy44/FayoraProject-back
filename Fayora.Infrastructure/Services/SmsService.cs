using Fayora.Application.Common.Interfaces.Services;

namespace Fayora.Infrastructure.Services;

public class SmsService : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string text)
    {
        throw new NotImplementedException();
    }
}
