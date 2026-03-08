using Fayora.Application.Common.Interfaces.Services;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Services.AuthServices;

public class MockWhatsAppService(IOptions<SmsSettings> options, ILogger<MockWhatsAppService> logger) : IWhatsAppService
{
    public Task SendWhatsAppMessageAsync(string phoneNumber, string message)
    {
        logger.LogWarning("🟢 [MOCK WHATSAPP] To: {Phone} | Message: {Message}", phoneNumber, message);

        return Task.CompletedTask;
    }
}