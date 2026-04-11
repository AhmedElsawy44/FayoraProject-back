using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AIModule;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Fayora.Api.Hubs;

public class AIHub(
    IAIService aiService,
    IUnitOfWork unitOfWork,
    IRecommendationService recoService,
    IUserRepository userRepository,
    ILogger<AIHub> logger
    ) : Hub
{
    public async Task SendUserMessage(string message)
    {
        try
        {
            var extractionResult = await aiService.ExtractSearchParametersAsync(message);

            var rawDbResults = await unitOfWork.SearchAsync(
                extractionResult.Intent,
                extractionResult.Params
            );

            var recommendedItems = await recoService.FilterAndRankAsync(rawDbResults, Context.UserIdentifier);
            string rawContextData = JsonSerializer.Serialize(recommendedItems);

            string userMetadata = "Guest";
            if (!string.IsNullOrEmpty(Context.UserIdentifier)
                && Guid.TryParse(Context.UserIdentifier, out var userId))
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                if (user != null)
                {
                    userMetadata = $"Name: {user.FirstName} {user.LastName}";
                }
            }

            var cancellationToken = Context.ConnectionAborted;

            await foreach (var chunk in aiService.GenerateFriendlyResponseAsync(message, rawContextData, userMetadata, cancellationToken))
            {
                await Clients.Caller.SendAsync("ReceiveAIChunk", chunk, cancellationToken);
            }

            await Clients.Caller.SendAsync("ReceiveMessageComplete", cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Connection aborted by user {UserId} during AI streaming.", Context.UserIdentifier);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing AI message for user {UserId}", Context.UserIdentifier);
            await Clients.Caller.SendAsync("ReceiveAIError", "عذراً، حدث خطأ أثناء معالجة طلبك، يرجى المحاولة لاحقاً.");
        }
    }
}