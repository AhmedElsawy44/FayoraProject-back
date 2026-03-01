using Fayora.Application.Common.Interfaces.Services;

namespace Fayora.Infrastructure.Services;

public class SmsService : ISmsService
{
    public Task SendSMSAsync(string phoneNumber, string text)
    {
        throw new NotImplementedException();
    }
}
