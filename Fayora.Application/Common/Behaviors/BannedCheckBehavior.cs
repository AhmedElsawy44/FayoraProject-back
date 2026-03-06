using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Common.Behaviors;

public class BannedCheckBehavior<TRequest, TResponse>(IDeviceRepository deviceRepository)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICheckBannedRequest
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (await deviceRepository.IsBannedAsync(request.DeviceId, cancellationToken))
        {
            return CreateErrorResult<TResponse>(AuthErrors.DeviceBanned);
        }

        return await next();
    }

    private static TResponse CreateErrorResult<T>(Error error) where T : Result
    {
        return (TResponse)Activator.CreateInstance(typeof(TResponse), error)!;
    }
}