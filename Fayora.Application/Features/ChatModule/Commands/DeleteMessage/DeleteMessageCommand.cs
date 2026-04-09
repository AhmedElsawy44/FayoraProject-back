using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.ChatModule.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid MessageId) : IRequest<Result<DeleteMessageResult>>;