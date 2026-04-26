using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid MessageId) : ICommand<Result<DeleteMessageResult>>;