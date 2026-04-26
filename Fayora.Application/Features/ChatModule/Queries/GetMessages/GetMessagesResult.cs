using Fayora.Domain.Entities.ChatModule;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public record GetMessagesResult(List<Message> Messages);