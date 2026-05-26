using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Services.Chatbot;

public class ChatbotServiceFactory : IChatbotServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ChatbotServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IChatbotService GetService(string provider)
    {
        var openRouterService = _serviceProvider.GetRequiredService<OpenRouterChatbotService>();
        var settings = _serviceProvider.GetRequiredService<IOptions<OpenRouterSettings>>().Value;

        string model = provider.ToLower() switch
        {
            "openai" => settings.OpenAIModel,
            "gemini" or _ => settings.GeminiModel
        };

        openRouterService.SetModel(model);
        return openRouterService;
    }
}
