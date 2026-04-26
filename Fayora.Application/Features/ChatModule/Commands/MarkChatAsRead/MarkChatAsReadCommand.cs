using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;

public record MarkChatAsReadCommand(Guid ChatId) : IRequest<Result<MarkChatAsReadResult>>;