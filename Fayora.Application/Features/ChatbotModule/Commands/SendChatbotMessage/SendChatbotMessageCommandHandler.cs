//using System;
//using System.Collections.Generic;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;
//using Fayora.Application.Common.Abstractions.Messaging;
//using Fayora.Application.Common.Interfaces.Persistences.ChatbotModule;
//using Fayora.Application.Common.Interfaces.Services.AuthModule;
//using Fayora.Application.Common.Interfaces.Services.ChatbotModule;
//using Fayora.Domain.Common.Results;
//using Fayora.Domain.Entities.ChatbotModule;

//namespace Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

//public class SendChatbotMessageCommandHandler(
//    IChatbotSessionRepository chatbotSessionRepository,
//    IAiChatService aiChatService,
//    ITravelRepository travelRepository,
//    IRecommendationEngineService recommendationEngineService,
//    IClientContextProvider clientContextProvider)
//    : ICommandHandler<SendChatbotMessageCommand, Result<ChatbotMessageResult>>
//{
//    public async Task<Result<ChatbotMessageResult>> Handle(
//        SendChatbotMessageCommand request,
//        CancellationToken cancellationToken)
//    {
//        var userIdResult = clientContextProvider.GetContext().UserId;
//        Guid? userId = userIdResult == Guid.Empty ? null : userIdResult;

//        Guid? sessionId = null!;
//        List<ChatMessageDto> history = [];
//        if (request.SessionId is not null)
//        {

//            history = await chatbotSessionRepository.GetSessionHistoryAsync(
//                request.SessionId.Value,
//                cancellationToken);
//        }
//        else
//        {
//            var session = ChatbotSession.Create(request.DeviceId, userId);
//            sessionId = session.Id;
//        }

//        // you must create sturctured class to fill not just string to make it more robust and avoid parsing errors
//        // see whole code for more details
//        string parametersJson = await aiChatService.ExtractSearchParametersAsync(
//            request.Content,
//            history,
//            cancellationToken);

//            // 3. Parsing: Deserialize JSON string returned from the AI into a C# DTO
//            TravelSearchParametersDto searchParams;
//            try
//            {
//                var jsonOptions = new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                };
//                searchParams = JsonSerializer.Deserialize<TravelSearchParametersDto>(parametersJson, jsonOptions)
//                               ?? new TravelSearchParametersDto(null, null, null, null);
//            }
//            catch (JsonException)
//            {
//                // Fallback to empty parameters if AI fails to return valid JSON
//                searchParams = new TravelSearchParametersDto(null, null, null, null);
//            }

//        // you replace this repo with booking & interactions etc
//            List<TravelOptionDto> travelOptions = await travelRepository.SearchTravelOptionsAsync(
//                searchParams,
//                cancellationToken);

//            TravelOptionDto? selectedOption = null;
//            if (travelOptions != null && travelOptions.Count > 0)
//            {
//                selectedOption = await recommendationEngineService.SelectBestOptionAsync(
//                    travelOptions,
//                    cancellationToken);
//            }

//            selectedOption ??= new TravelOptionDto(
//                Guid.Empty,
//                "No specific matches found",
//                "We couldn't find a direct matching option for this query.",
//                searchParams.Destination ?? "Egypt",
//                0,
//                0.0,
//                "None"
//            );
//        // allow humanizerecommadation to take more than one items and create more conversational response with more details about the options and why they are recommended
//        string humanizedResponse = await aiChatService.HumanizeRecommendationAsync(
//                selectedOption,
//                cancellationToken);


//            var responseJson = JsonSerializer.Serialize(new { text = humanizedResponse });
//            var result = new ChatbotMessageResult(sessionId.Value, responseJson);

//            return Result<ChatbotMessageResult>.CreateSuccess(result);
//    }
//}
