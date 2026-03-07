using Fayora.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Services.AuthServices;

public class MockSmsService(IOptions<SmsSettings> smsSettings, ILogger<MockSmsService> logger) : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string text)
    {
        logger.LogWarning("🟢 [MOCK SmsService To: {Phone} | Message: {text}", phoneNumber, text);
        return Task.CompletedTask;
    }
}
