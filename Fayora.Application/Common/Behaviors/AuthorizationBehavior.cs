using Fayora.Application.Common.Authorization;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Domain.Common.Results;
using MediatR;
using System.Reflection;

namespace Fayora.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IClientContextProvider clientContextProvider)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizationAttributes.Count == 0)
        {
            return await next();
        }

        var currentUser = clientContextProvider.GetContext();

        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
            .ToList();

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        return await next();
    }
}
