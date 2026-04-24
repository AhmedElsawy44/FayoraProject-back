using MediatR;

namespace Fayora.Application.Common.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}