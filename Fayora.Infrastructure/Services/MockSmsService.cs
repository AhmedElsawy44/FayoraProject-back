using Fayora.Application.Common.Interfaces.Services;

namespace Fayora.Infrastructure.Services;

public class MockSmsService : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string text)
    {
        Console.WriteLine($"Mock SMS sent to {phoneNumber}: {text}");
        return Task.CompletedTask;
    }
}
