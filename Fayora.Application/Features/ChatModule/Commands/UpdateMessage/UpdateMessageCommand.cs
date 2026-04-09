using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.ChatModule.Commands.UpdateMessage;

public record UpdateMessageCommand(Guid MessageId, string NewContent) : IRequest<Result<UpdateMessageResult>>;
